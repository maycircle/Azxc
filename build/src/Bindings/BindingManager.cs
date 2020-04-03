using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DuckGame;

namespace Azxc.Bindings
{
    public class BindingManager : IEnumerable<IAutoBinding>, IAutoUpdate
    {
        private List<IAutoBinding> _bindings;

        public BindingManager()
        {
            _bindings = new List<IAutoBinding>();
        }

        public BindingManager(List<IAutoBinding> bindings)
        {
            _bindings = bindings;
        }

        public IEnumerator<IAutoBinding> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        public void Add(IAutoBinding binding)
        {
            _bindings.Add(binding);
        }

        public static void UsedBinding(IBinding original, string methodName)
        {
            MethodInfo method = original.GetType().GetMethod(methodName);

            object[] attributes = method.GetCustomAttributes(true);
            BindingAttribute binding = null;
            foreach (object attribute in attributes)
            {
                if (attribute is BindingAttribute)
                    binding = attribute as BindingAttribute;
            }
            if (binding == null)
                return;

            if (binding.UsedBinding())
                method.Invoke(original, null);
        }

        public static void UsedBinding(IBinding original, string methodName, BindingAttribute binding)
        {
            MethodInfo method = original.GetType().GetMethod(methodName);
            if (binding.UsedBinding())
                method.Invoke(original, null);
        }

        public void Update()
        {
            foreach (IAutoBinding binding in _bindings)
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
                            if (found.UsedBinding())
                                method.Invoke(binding, null);
                        }
                    }
                }
            }
        }
    }
}
