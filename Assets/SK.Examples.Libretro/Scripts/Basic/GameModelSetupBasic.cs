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

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    public sealed class GameModelSetupBasic : GameModelSetup
    {
        protected override void OnLateStart()
        {
            //InputEnabled = true;
        }

        protected override void OnUpdate()
        {
            if (Keyboard.current is null)
                return;

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (Paused)
                    Resume();
                else
                    Pause();
            }

            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                SaveState(0);
                Debug.Log("State saved to slot 0");
            }
            if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                SaveState(1);
                Debug.Log("State saved to slot 1");
            }
            if (Keyboard.current.f7Key.wasPressedThisFrame)
            {
                SaveState(2);
                Debug.Log("State saved to slot 2");
            }
            if (Keyboard.current.f8Key.wasPressedThisFrame)
            {
                SaveState(3);
                Debug.Log("State saved to slot 3");
            }

            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                LoadState(0);
                Debug.Log("State loaded from slot 0");
            }
            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                LoadState(1);
                Debug.Log("State loaded from slot 1");
            }
            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                LoadState(2);
                Debug.Log("State loaded from slot 2");
            }
            if (Keyboard.current.f12Key.wasPressedThisFrame)
            {
                LoadState(3);
                Debug.Log("State loaded from slot 3");
            }

            //Rewind(Keyboard.current.backspaceKey.isPressed);
        }

        protected override string ConfigFilePath { get; } = Application.streamingAssetsPath + "/config_basic.json";
        protected override ConfigFileContent LoadJsonConfig(string json) => JsonUtility.FromJson<ConfigFileContent>(json);
        protected override string GetJsonConfig() => JsonUtility.ToJson(new ConfigFileContent(this), true);
    }
}
