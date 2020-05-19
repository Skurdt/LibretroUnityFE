/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleWindows : IDllModule
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr PlatformLoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr PlatformGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool PlatformFreeLibrary(IntPtr hModule);

        public string Name { get; private set; }
        public string Extension { get; } = "dll";
        public IntPtr NativeHandle { get; private set; }

        public void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Name         = Path.GetFileName(path);
                NativeHandle = PlatformLoadLibrary(path);
                if (NativeHandle == IntPtr.Zero)
                {
                    throw new Exception($"Failed to load library at path '{path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
                }
            }
            else
            {
                throw new Exception("Library path is null or empty.");
            }
        }

        public T GetFunction<T>(string functionName) where T : Delegate
        {
            if (NativeHandle != IntPtr.Zero)
            {
                IntPtr procAddress = PlatformGetProcAddress(NativeHandle, functionName);
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

        public void Free()
        {
            if (NativeHandle != IntPtr.Zero)
            {
                _ = PlatformFreeLibrary(NativeHandle);
            }
        }
    }
}
