using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DuckGame;

namespace Azxc
{
    public class NMCallDuck : NMEvent
    {
        public byte index;
        public string method;
        public object[] parameters;

        private byte _levelIndex;

        public NMCallDuck()
        {
            // Empty :/
        }

        public NMCallDuck(byte idx, string method, object[] parameters)
        {
            index = idx;
            this.method = method;
            this.parameters = parameters;
        }

        public override void Activate()
        {
            Profile profile = DuckNetwork.profiles[index];
            Duck target = profile.duck;

            if (target == null || !(Level.current is GameLevel) ||
                DuckNetwork.levelIndex != _levelIndex)
                return;

            foreach (MethodInfo method in target.GetType().GetMethods())
            {
                if (method.Name == this.method)
                    method.Invoke(target, parameters);
            }

            Thing.Fondle(target, connection);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
        }
    }
}
