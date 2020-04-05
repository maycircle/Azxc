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
        private UserInterfaceInteract _interact;

        private static float _resolution;

        public UserInterfaceState state
        {
            get { return _core.state; }
        }

        public List<Control> controls
        {
            get { return _core.controls; }
        }

        public Cursor cursor
        {
            get { return _core.cursor; }
        }

        public FancyBitmapFont font
        {
            get { return _core.font; }
        }

        public UserInterfaceManager(UserInterfaceState state)
        {
            _core = new UserInterfaceCore();
            _core.state = state;

            _interact = new UserInterfaceInteract();

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

            _interact.Update();

            _core.cursor.scale = new Vec2(_resolution / 2f);
            _core.cursor.position = Mouse.position;
            _core.cursor.Update();

            foreach (Control control in _core.controls.OfType<IAutoUpdate>())
            {
                IAutoUpdate updatable = control as IAutoUpdate;
                updatable.Update();
            }
        }

        public void Draw()
        {
            if (!_core.state.HasFlag(UserInterfaceState.Open))
                return;

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
