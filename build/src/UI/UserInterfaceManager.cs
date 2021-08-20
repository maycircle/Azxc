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

        private BitmapFont _hintsFont;
        public string hintsText { get; private set; }
        public bool forceHints { get; set; }

        public bool isKeyboardLocked { get; private set; }

        // Short-cuts

        public Cursor cursor => _core.cursor;
        public FancyBitmapFont font => _core.font;

        public UserInterfaceInteract GetInteractionManager() => _core.interact;
        public UserInterfaceState GetState() => _core.state;
        public List<IAutoUpdate> GetUpdatable() => _core.GetUpdatable();

        private static void OnLevelLoad(Level __instance)
        {
            // Due to problems with UI scaling, it will calculate a (mostly) perfect resolution for
            // interface
            _resolution = __instance.camera.width / 320f;
        }

        public UserInterfaceManager(UserInterfaceState state)
        {
            _core = new UserInterfaceCore();
            _core.state = state;

            _hintsFont = new BitmapFont("biosFont", 8);

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
            float width = _hintsFont.GetWidth(hintsText);

            Vec2 scale = new Vec2(0.5f);
            _hintsFont.scale = scale;
            _hintsFont.spriteScale = scale;
            width *= scale.x;

            Vec2 cornerIndent = new Vec2(45.0f, _hintsFont.height * 2.0f);
            Vec2 position = new Vec2(Layer.HUD.width - width - cornerIndent.x,
                Layer.HUD.height - _hintsFont.height - cornerIndent.y);

            _hintsFont.DrawOutline(hintsText, position, Color.White, Color.Black, 0.8f);
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

        public void SetHintsText(string text)
        {
            hintsText = text;
        }

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
