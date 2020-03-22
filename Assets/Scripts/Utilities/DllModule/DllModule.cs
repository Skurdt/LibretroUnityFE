using System;

namespace SK.Utilities
{
    public abstract class DllModule
    {
        public string Name { get; protected set; } = string.Empty;
        public IntPtr NativeHandle { get; protected set; } = IntPtr.Zero;

        public abstract T GetFunction<T>(string functionName) where T : Delegate;
        public abstract void Free();
    }
}
