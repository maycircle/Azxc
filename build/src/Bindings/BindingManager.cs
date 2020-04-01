using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DuckGame;

namespace Azxc.Bindings
{
    public class BindingManager : IEnumerable<IBinding>, IAutoUpdate
    {
        private List<IBinding> _bindings;

        public BindingManager()
        {
            _bindings = new List<IBinding>();
        }

        public BindingManager(List<IBinding> bindings)
        {
            _bindings = bindings;
        }

        public IEnumerator<IBinding> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        public void Add(IBinding binding)
        {
            _bindings.Add(binding);
        }

        public void Update()
        {
            foreach (IBinding binding in _bindings)
            {
                MethodInfo[] methods = binding.GetType().GetMethods();
                foreach (MethodInfo method in methods)
                {
                    object[] attributes = method.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        if (attribute is BindingAttribute)
                        {
                            BindingAttribute found = (attribute as BindingAttribute);
                            if (Keyboard.Pressed(found.key))
                                method.Invoke(binding, null);
                        }
                    }
                }
            }
        }
    }
}
