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
        private UserInterfaceState _state;
        private Queue<Control> _controls;

        private Cursor _cursor;

        private static float resolution;

        public UserInterfaceState state
        {
            get { return _state; }
        }

        public UserInterfaceManager(UserInterfaceState state)
        {
            _state = state;
            _controls = new Queue<Control>();

            _cursor = new Cursor(1f, Vec2.One);

            // So our GUI will draw everywhere
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
            resolution = __instance.camera.width / 320f;
        }

        [Binding(Keys.Insert)]
        public void Open()
        {
            if (_state.HasFlag(UserInterfaceState.Open))
            {
                _state &= ~UserInterfaceState.Open;
                _state |= UserInterfaceState.Freeze;
            }
            else
            {
                _state |= UserInterfaceState.Open;
                _state &= ~UserInterfaceState.Freeze;
            }
        }

        public void Update()
        {
            if (!_state.HasFlag(UserInterfaceState.Enabled))
                return;

            BindingManager.UsedBinding(this, "Open");

            if (_state.HasFlag(UserInterfaceState.Freeze))
                return;

            _cursor.scale = new Vec2(resolution / 2f);
            _cursor.position = Mouse.position;
            _cursor.Update();

            foreach (Control control in _controls.OfType<IAutoUpdate>())
            {
                IAutoUpdate updatable = control as IAutoUpdate;
                updatable.Update();
            }
        }

        public void Draw()
        {
            if (!_state.HasFlag(UserInterfaceState.Open))
                return;

            _cursor.Draw();

            foreach (Control control in _controls)
            {
                control.Draw();
            }
        }
    }
}
