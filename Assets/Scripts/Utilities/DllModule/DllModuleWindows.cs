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

        public DllModuleWindows(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = Win32LoadLibrary(path);
                if (hModule != IntPtr.Zero)
                {
                    Name = Path.GetFileName(path);
                    NativeHandle = hModule;
                }
                else
                {
                    throw new Exception($"Failed to load library at path '{path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
                }
            }
            else
            {
                throw new Exception($"Library path is null or empty.");
            }
        }

        public override T GetFunction<T>(string functionName)
        {
            if (NativeHandle != IntPtr.Zero)
            {
                IntPtr procAddress = Win32GetProcAddress(NativeHandle, functionName);
                if (procAddress != IntPtr.Zero)
                {
                    return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
                }
                else
                {
                    throw new Exception($"Function '{functionName}' not found in library '{Name}'.");
                }
            }
            else
            {
                throw new Exception($"Library not loaded, cannot get function '{functionName}'");
            }
        }

        public override void Free()
        {
            if (NativeHandle != IntPtr.Zero)
            {
                _ = Win32FreeLibrary(NativeHandle);
            }
        }
    }
}
