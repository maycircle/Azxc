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
        private UserInterfaceCore core;
        private UserInterfaceInteract interact;

        private static float resolution;

        public UserInterfaceState state
        {
            get { return core.state; }
        }

        public List<Control> controls
        {
            get { return core.controls; }
        }

        public Cursor cursor
        {
            get { return core.cursor; }
        }

        public FancyBitmapFont font
        {
            get { return core.font; }
        }

        public UserInterfaceManager(UserInterfaceState state)
        {
            core = new UserInterfaceCore();
            core.state = state;

            interact = new UserInterfaceInteract();

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

        [Binding(Keys.Insert, InputState.Pressed)]
        public void Open()
        {
            if (core.state.HasFlag(UserInterfaceState.Open))
            {
                core.state &= ~UserInterfaceState.Open;
                core.state |= UserInterfaceState.Freeze;
            }
            else
            {
                core.state |= UserInterfaceState.Open;
                core.state &= ~UserInterfaceState.Freeze;
            }
        }

        public void Update()
        {
            if (!core.state.HasFlag(UserInterfaceState.Enabled))
                return;

            interact.Update();

            BindingManager.UsedBinding(this, "Open");

            if (core.state.HasFlag(UserInterfaceState.Freeze))
                return;

            core.cursor.scale = new Vec2(resolution / 2f);
            core.cursor.position = Mouse.position;
            core.cursor.Update();

            foreach (Control control in core.controls.OfType<IAutoUpdate>())
            {
                IAutoUpdate updatable = control as IAutoUpdate;
                updatable.Update();
            }
        }

        public void Draw()
        {
            if (!core.state.HasFlag(UserInterfaceState.Open))
                return;

            core.cursor.Draw();

            foreach (Control control in core.controls)
            {
                control.Draw();
            }
        }

        public void AddControl(Control control)
        {
            core.controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            core.controls.Remove(control);
        }
    }
}
