using System;

namespace SK.Libretro.Utilities
{
    public abstract class DllModule
    {
        public string Name { get; protected set; } = string.Empty;

        protected IntPtr _nativeHandle = IntPtr.Zero;

        public abstract bool Load(string path);
        public abstract T GetFunction<T>(string functionName) where T : Delegate;
        public abstract void Free();
    }
}
