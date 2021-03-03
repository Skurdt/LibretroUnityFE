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
using UnityEngine;

namespace SK.Examples
{
    public sealed class GameModelSetupInteractive : GameModelSetup
    {
        [Serializable]
        private sealed class ConfigFileContentList
        {
            public ConfigFileContent[] Entries;
            public ConfigFileContentList(int length) => Entries = new ConfigFileContent[length];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CameraTriggerObject camera))
            {
                camera.HeadSync = false;
                InputEnabled    = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CameraTriggerObject camera))
            {
                camera.HeadSync = true;
                InputEnabled    = false;
            }
        }

        protected override void OnLateStart()
        {
            if (_menu != null)
            {
                _menu.ShowCommonOptions();
                _menu.HideCoreSpecificOptions();
            }
        }

        protected override string ConfigFilePath => Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "config_interactive.json"));

        protected override ConfigFileContent LoadJsonConfig(string json)
        {
            ConfigFileContentList gameList = JsonUtility.FromJson<ConfigFileContentList>(json);
            return gameList.Entries == null || gameList.Entries.Length == 0 || transform.GetSiblingIndex() > gameList.Entries.Length - 1
                ? null
                : gameList.Entries[transform.GetSiblingIndex()];
        }

        protected override string GetJsonConfig()
        {
            GameModelSetup[] gameModelSetups = transform.parent.GetComponentsInChildren<GameModelSetup>();
            if (gameModelSetups.Length == 0)
                return null;

            ConfigFileContentList gameList = new ConfigFileContentList(gameModelSetups.Length);
            for (int i = 0; i < gameModelSetups.Length; ++i)
                gameList.Entries[i] = new ConfigFileContent(gameModelSetups[i]);

            return JsonUtility.ToJson(gameList, true);
        }
    }
}
