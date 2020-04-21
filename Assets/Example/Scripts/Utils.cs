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

namespace SK.Examples.Common
{
    public static class Utils
    {
        public static bool SpaceKeyPressed() => Keyboard.current.spaceKey.wasPressedThisFrame;
        public static bool LShiftKeyDown() => Keyboard.current.leftShiftKey.isPressed;

        public static bool ToggleMouseCursor()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return ShowMouseCursor();
            }
            else
            {
                return HideMouseCursor();
            }
        }

        public static bool ShowMouseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
            return true;
        }

        public static bool HideMouseCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
            return false;
        }

        public static void ExitApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(0);
#endif
        }
    }
}
