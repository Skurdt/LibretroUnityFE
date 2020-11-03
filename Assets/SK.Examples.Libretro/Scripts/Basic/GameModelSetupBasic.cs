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

using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    internal sealed class GameModelSetupBasic : GameModelSetup
    {
        protected override void OnUpdate()
        {
            if (_libretro == null || !_libretro.Running)
                return;

            if (Keyboard.current == null)
                return;

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (_libretro.Running)
                    _libretro.Pause();
                else
                    _libretro.Resume();
            }

            if (Keyboard.current.f5Key.wasPressedThisFrame && SaveState(0))
                Debug.Log("State saved to slot 0");
            if (Keyboard.current.f6Key.wasPressedThisFrame && SaveState(1))
                Debug.Log("State saved to slot 1");
            if (Keyboard.current.f7Key.wasPressedThisFrame && SaveState(2))
                Debug.Log("State saved to slot 2");
            if (Keyboard.current.f8Key.wasPressedThisFrame && SaveState(3))
                Debug.Log("State saved to slot 3");

            if (Keyboard.current.f9Key.wasPressedThisFrame && LoadState(0))
                Debug.Log("State loaded from slot 0");
            if (Keyboard.current.f10Key.wasPressedThisFrame && LoadState(1))
                Debug.Log("State loaded from slot 1");
            if (Keyboard.current.f11Key.wasPressedThisFrame && LoadState(2))
                Debug.Log("State loaded from slot 2");
            if (Keyboard.current.f12Key.wasPressedThisFrame && LoadState(3))
                Debug.Log("State loaded from slot 3");

            _libretro.Rewind(Keyboard.current.backspaceKey.isPressed);
        }
    }
}
