using Azxc.UI.Controls;
using DuckGame;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Azxc.UI
{
    internal class UserInterfaceManager_DoDraw
    {
        static void Postfix()
        {
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone,
                null, Layer.HUD.camera.getMatrix());
            Azxc.GetCore().GetUI().Draw();
            Graphics.screen.End();
        }
    }

    public class UserInterfaceManager : IAutoUpdate
    {
        private UserInterfaceCore _core;

        private static float _resolution;

        public string hintsText { get; set; }
        public bool forceHints { get; set; }

        public bool isKeyboardLocked { get; private set; }

        // Short-cuts

        public Cursor cursor => _core.cursor;
        public FancyBitmapFont font => _core.font;

        public UserInterfaceManager(UserInterfaceState state)
        {
            _core = new UserInterfaceCore();
            _core.state = state;

            // So the GUI will be drawn everywhere
            Azxc.GetCore().GetHarmony().Patch(typeof(Level).GetMethod("DoDraw"),
                postfix: new HarmonyMethod(typeof(UserInterfaceManager_DoDraw), "Postfix"));

            // Getting Level's class constructor, so OnLevelLoad would be called each time new
            // instance of Level is created
            ConstructorInfo ctor = typeof(Level).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis,
                new Type[] { }, null);
            Azxc.GetCore().GetHarmony().Patch(ctor, postfix: new HarmonyMethod(typeof(UserInterfaceManager),
                "OnLevelLoad"));
        }

        private static void OnLevelLoad(Level __instance)
        {
            // Due to problems with UI scaling, it will calculate a (mostly) perfect resolution for
            // interface
            _resolution = __instance.camera.width / 320f;
        }

        public void Open()
        {
            if (_core.state.HasFlag(UserInterfaceState.Open))
            {
                _core.state &= ~UserInterfaceState.Open;
                _core.state |= UserInterfaceState.Freeze;
            }
            else
            {
                _core.state |= UserInterfaceState.Open;
                _core.state &= ~UserInterfaceState.Freeze;

                foreach (Control control in _core.GetUpdatable().OfType<Control>())
                    control.Appear();
            }
        }

        public void Update()
        {
            if (!_core.state.HasFlag(UserInterfaceState.Enabled))
                return;

            if (Keyboard.Pressed(Keys.Insert))
                Open();

            bool freezed = _core.state.HasFlag(UserInterfaceState.Freeze);

            if (!freezed)
            {
                _core.interact.Update();
                _core.cursor.scale = new Vec2(_resolution / 2f);
                _core.cursor.position = Mouse.position;
                _core.cursor.Update();
            }

            foreach (IAutoUpdate item in _core.GetUpdatable())
            {
                if (!freezed || Attribute.IsDefined(item.GetType(), typeof(ForceUpdateAttribute)))
                    item.Update();
            }

            if (!freezed && _core.interact.activeWindow is IDialog)
            {
                IDialog dialog = _core.interact.activeWindow as IDialog;
                if (dialog.dialogResult != DialogResult.Idle)
                {
                    _core.interact.activeWindow.Close();
                    dialog.ThrowResult();
                }
            }
        }

        public void DrawHints()
        {
            BitmapFont bitmapFont = new BitmapFont("biosFont", 8);
            float width = bitmapFont.GetWidth(hintsText);
            // BitmapFont's GetWidth method doesn't calculate right spriteScale field change, so I
            // calculate the full width at scale 1x, and then just multiply it by new scale (knowing
            // that scale and spriteScale are equal)
            Vec2 scale = new Vec2(0.5f);
            bitmapFont.scale = scale;
            bitmapFont.spriteScale = scale;
            width *= scale.x;

            Vec2 cornerIndent = new Vec2(15.0f, bitmapFont.height * 2.0f);
            Vec2 position = new Vec2(Layer.HUD.width - width - cornerIndent.x,
                Layer.HUD.height - bitmapFont.height - cornerIndent.y);

            bitmapFont.DrawOutline(hintsText, position, Color.White, Color.Black, 0.8f);
        }

        public void Draw()
        {
            if (!_core.state.HasFlag(UserInterfaceState.Open))
                return;

            if (_core.interact.activeWindow.GetType() == typeof(MainWindow) || forceHints)
                DrawHints();
            _core.cursor.Draw();

            foreach (Control control in _core.GetUpdatable().OfType<Control>())
                control.Draw();
        }

        public UserInterfaceInteract GetInteractionManager() => _core.interact;
        public UserInterfaceState GetState() => _core.state;
        public List<IAutoUpdate> GetUpdatable() => _core.GetUpdatable();

        public void SetKeyboardLock(bool state)
        {
            KeyboardHook.HookAndToggle(state && _core.state.HasFlag(UserInterfaceState.Open));
            isKeyboardLocked = state;
        }

        public void AddUpdatable(IAutoUpdate updatable)
        {
            _core.AddUpdatable(updatable);
        }

        public void RemoveUpdatable(IAutoUpdate updatable)
        {
            _core.RemoveUpdatable(updatable);
        }
    }
}
