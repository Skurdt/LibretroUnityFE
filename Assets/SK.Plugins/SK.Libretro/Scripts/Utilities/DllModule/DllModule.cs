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
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public abstract class DllModule
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public string Extension { get; private set; }

        protected IntPtr _nativeHandle;

        protected DllModule(string extension) => Extension = extension;

        public void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Library path is null or empty.");
            }

            Path = System.IO.Path.GetFullPath(path);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);

            try
            {
                LoadLibrary();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetFunction<T>(string functionName) where T : Delegate
        {
            if (_nativeHandle == IntPtr.Zero)
            {
                throw new Exception($"Library '{Name}' not loaded, cannot get function '{functionName}'");
            }

            try
            {
                IntPtr procAddress = GetProcAddress(functionName);
                if (procAddress == IntPtr.Zero)
                {
                    throw new Exception($"Function '{functionName}' not found in library '{Name}' at path '{Path}'");
                }

                return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Free()
        {
            if (_nativeHandle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                FreeLibrary();
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected abstract void LoadLibrary();

        protected abstract IntPtr GetProcAddress(string functionName);

        protected abstract void FreeLibrary();
    }
}
