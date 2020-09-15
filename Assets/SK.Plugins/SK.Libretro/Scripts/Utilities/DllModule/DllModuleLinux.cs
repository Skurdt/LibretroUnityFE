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
    public sealed class DllModuleLinux : IDllModule
    {
        [DllImport("libdl.so", EntryPoint = "dlopen", SetLastError = true)]
        private static extern IntPtr PlatformLoadLibrary(string fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlsym", SetLastError = true)]
        private static extern IntPtr PlatformGetProcAddress(IntPtr handle, string symbol);

        [DllImport("libdl.so", EntryPoint = "dlclose", SetLastError = true)]
        private static extern int PlatformFreeLibrary(IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlerror")]
        private static extern IntPtr PlatformLibraryError();

        public string Name { get; private set; }
        public string Extension { get; } = "so";
        public IntPtr NativeHandle { get; private set; }

        private const int RTLD_NOW = 2;

        public void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Name         = Path.GetFileName(path);
                NativeHandle = PlatformLoadLibrary(path, RTLD_NOW);
                IntPtr error = PlatformLibraryError();
                if (error != IntPtr.Zero)
                {
                    throw new Exception($"Failed to load library at path '{path}' (ErrorMessage: {Marshal.PtrToStringAnsi(error)})");
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
