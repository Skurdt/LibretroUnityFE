using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Utilities
{
    public sealed class DllModuleWindows : DllModule
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr Win32LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr Win32GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool Win32FreeLibrary(IntPtr hModule);

        public override bool Load(string path)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = Win32LoadLibrary(path);
                if (hModule != IntPtr.Zero)
                {
                    Name          = Path.GetFileName(path);
                    _nativeHandle = hModule;
                    result = true;
                }
                else
                {
                    Log.Error($"Failed to load library at path '{path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
                }
            }
            else
            {
                Log.Error($"Library path is null or empty.");
            }

            return result;
        }

        public override bool GetFunction<T>(string functionName, out T functionPtr)
        {
            bool result = false;
            functionPtr = null;

            if (_nativeHandle != IntPtr.Zero)
            {
                IntPtr procAddress = Win32GetProcAddress(_nativeHandle, functionName);
                if (procAddress != IntPtr.Zero)
                {
                    functionPtr = Marshal.GetDelegateForFunctionPointer<T>(procAddress);
                    result = true;
                }
                else
                {
                    Log.Error($"Function '{functionName}' not found in library '{Name}'.");
                }
            }
            else
            {
                Log.Error($"Library not loaded, cannot get function '{functionName}'");
            }

            return result;
        }

        public override void Free()
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                _ = Win32FreeLibrary(_nativeHandle);
            }
        }
    }
}
