/* MIT License

 * Copyright (c) 2021-2022 Skurdt
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

using SK.Libretro.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [ExecuteAlways, DisallowMultipleComponent, DefaultExecutionOrder(-3)]
    public sealed class JsonConfigLoader : MonoBehaviour
    {
        public Transform InstancesParent;

        [Serializable]
        private sealed class ConfigFileContent
        {
            public string CoreName;
            public string GamesDirectory;
            public List<string> GameNames;

            public ConfigFileContent(LibretroInstance libretroInstance)
            {
                CoreName       = libretroInstance.CoreName;
                GamesDirectory = libretroInstance.GamesDirectory;
                GameNames      = libretroInstance.GameNames.ToList();
            }

            public void Update(LibretroInstance libretroInstance)
            {
                CoreName       = libretroInstance.CoreName;
                GamesDirectory = libretroInstance.GamesDirectory;
                GameNames      = libretroInstance.GameNames.ToList();
            }
        }

        [Serializable]
        private sealed class ConfigFileContentList
        {
            public List<ConfigFileContent> Entries = new();
        }

        private static readonly string _configFilePath = Application.streamingAssetsPath + "/GamesSetup.json";

        private void OnEnable() => LoadConfig();

        public void LoadConfig()
        {
            Transform instancesParent = InstancesParent != null ? InstancesParent : transform;
            List<LibretroInstance> libretroInstances = GetLibretroInstances(instancesParent);
            if (libretroInstances.Count == 0)
                return;

            if (!FileSystem.FileExists(_configFilePath))
                return;

            string json = File.ReadAllText(_configFilePath);
            if (string.IsNullOrEmpty(json))
                return;

            ConfigFileContentList contentList = JsonUtility.FromJson<ConfigFileContentList>(json);
            if (contentList is null || contentList.Entries is null || contentList.Entries.Count == 0)
                return;

            for (int i = 0; i < libretroInstances.Count; ++i)
            {
                LibretroInstance libretroInstance = libretroInstances[i];

                if (contentList.Entries.Count <= i)
                {
                    libretroInstance.DeInitialize();
                    continue;
                }

                ConfigFileContent content = contentList.Entries[i];
                libretroInstance.Initialize(content.CoreName, content.GamesDirectory, content.GameNames.ToArray());
            }
        }

        public void SaveConfig()
        {
            Transform instancesParent = InstancesParent != null ? InstancesParent : transform;
            List<LibretroInstance> libretroInstances = GetLibretroInstances(instancesParent);
            if (libretroInstances.Count == 0)
                return;

            ConfigFileContentList contentList = new();
            if (FileSystem.FileExists(_configFilePath))
            {
                string loadedJson = File.ReadAllText(_configFilePath);
                if (!string.IsNullOrEmpty(loadedJson))
                    contentList = JsonUtility.FromJson<ConfigFileContentList>(loadedJson);
            }

            for (int i = 0; i < libretroInstances.Count; ++i)
            {
                if (contentList.Entries.Count > i)
                    contentList.Entries[i].Update(libretroInstances[i]);
                else
                    contentList.Entries.Add(new (libretroInstances[i]));
            }

            string json = JsonUtility.ToJson(contentList, true);
            if (string.IsNullOrEmpty(json))
                return;

            File.WriteAllText(_configFilePath, json);
        }

        private static List<LibretroInstance> GetLibretroInstances(Transform parent)
        {
            List<LibretroInstance> libretroInstances = new();

            if (parent.TryGetComponent(out LibretroInstance libretroInstance))
                libretroInstances.Add(libretroInstance);

            foreach (Transform child in parent)
                libretroInstances.AddRange(GetLibretroInstances(child));

            return libretroInstances;
        }
    }
}
