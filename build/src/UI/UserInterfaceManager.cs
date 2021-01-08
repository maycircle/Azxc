using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;
using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    internal class UserInterfaceManager_DoDraw
    {
        static void Postfix()
        {
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone,
                null, Layer.HUD.camera.getMatrix());
            Azxc.core.uiManager.Draw();
            Graphics.screen.End();
        }
    }

    public class UserInterfaceManager : IAutoUpdate, IBinding
    {
        private UserInterfaceCore _core;

        private static float _resolution;

        // Short-cuts

        public UserInterfaceInteract interact => _core.interact;
        public UserInterfaceState state => _core.state;
        public List<Control> controls => _core.controls;
        public Cursor cursor => _core.cursor;
        public FancyBitmapFont font => _core.font;

        public UserInterfaceManager(UserInterfaceState state)
        {
            _core = new UserInterfaceCore();
            _core.state = state;

            // So the GUI will be drawn everywhere
            Azxc.core.harmony.Patch(typeof(Level).GetMethod("DoDraw"),
                postfix: new HarmonyMethod(typeof(UserInterfaceManager_DoDraw), "Postfix"));

            // Getting Level's class constructor, so OnLevelLoad would be called each time new
            // instance of Level is created
            ConstructorInfo ctor = typeof(Level).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis,
                new Type[] { }, null);
            Azxc.core.harmony.Patch(ctor, postfix: new HarmonyMethod(typeof(UserInterfaceManager),
                "OnLevelLoad"));
        }

        // The method would be called each time any level loads
        private static void OnLevelLoad(Level __instance)
        {
            // Due to problems with UI scaling, it will calculate a (mostly) perfect resolution for
            // interface
            _resolution = __instance.camera.width / 320f;
        }

        [Binding(Keys.Insert, InputState.Pressed)]
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
            }
        }

        public void Update()
        {
            if (!_core.state.HasFlag(UserInterfaceState.Enabled))
                return;

            BindingManager.UsedBinding(this, "Open");

            if (_core.state.HasFlag(UserInterfaceState.Freeze))
                return;

            _core.interact.Update();

            _core.cursor.scale = new Vec2(_resolution / 2f);
            _core.cursor.position = Mouse.position;
            _core.cursor.Update();

            foreach (Control control in _core.controls.OfType<IAutoUpdate>())
            {
                IAutoUpdate updatable = control as IAutoUpdate;
                updatable.Update();
            }
        }

        public void DrawHints()
        {
            BitmapFont bitmapFont = new BitmapFont("biosFont", 8);
            Vec2 scale = new Vec2(0.5f);
            bitmapFont.scale = scale;
            bitmapFont.spriteScale = scale;

            string text = "@AZXCACTIVATE@ACTIVATE @AZXCBACK@BACK";
            float width = bitmapFont.GetWidth(text);

            Vec2 cornerIndent = new Vec2(0.0f, bitmapFont.height * 2.0f);
            Vec2 position = new Vec2(Layer.HUD.width - width - cornerIndent.x,
                Layer.HUD.height - bitmapFont.height - cornerIndent.y);

            bitmapFont.DrawOutline(text, position, Color.White, Color.Black, 0.8f);
        }

        public void Draw()
        {
            if (!_core.state.HasFlag(UserInterfaceState.Open))
                return;

            DrawHints();
            _core.cursor.Draw();

            foreach (Control control in _core.controls)
            {
                control.Draw();
            }
        }

        public void AddControl(Control control)
        {
            _core.controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            _core.controls.Remove(control);
        }
    }
}
