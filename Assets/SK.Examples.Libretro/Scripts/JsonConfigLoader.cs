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

using SK.Libretro.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SK.Examples
{
    public sealed class JsonConfigLoader : MonoBehaviour
    {
        public Transform InstancesParent;

        [Serializable]
        private sealed class ConfigFileContent
        {
            public string CoreName;
            public string GamesDirectory;
            public string[] GameNames;
            public ConfigFileContent(LibretroInstance libretroInstance)
            {
                CoreName       = libretroInstance.CoreName;
                GamesDirectory = libretroInstance.GamesDirectory;
                GameNames      = libretroInstance.GameNames;
            }
        }

        [Serializable]
        private sealed class ConfigFileContentList
        {
            public ConfigFileContent[] Entries;
            public ConfigFileContentList(int length) => Entries = new ConfigFileContent[length];
        }

        private static readonly string _configFilePath = Application.streamingAssetsPath + "/GamesSetup.json";

        private void Start() => LoadConfig();

        public void LoadConfig()
        {
            Transform instancesParent = InstancesParent != null ? InstancesParent : transform;
            List<LibretroInstance> libretroInstances = GetLibretroInstances(instancesParent);
            if (libretroInstances.Count == 0)
                return;

            if (!File.Exists(_configFilePath))
                return;

            string json = File.ReadAllText(_configFilePath);
            if (string.IsNullOrEmpty(json))
                return;

            ConfigFileContentList contentList = JsonUtility.FromJson<ConfigFileContentList>(json);
            if (contentList == null || contentList.Entries == null || contentList.Entries.Length == 0)
                return;

            for (int i = 0; i < libretroInstances.Count; ++i)
            {
                if (contentList.Entries.Length <= i)
                {
                    libretroInstances[i].SetContent(null, null, null);
                    continue;
                }

                ConfigFileContent content = contentList.Entries[i];
                libretroInstances[i].SetContent(content.CoreName, content.GamesDirectory, content.GameNames);
            }
        }

        public void SaveConfig()
        {
            Transform instancesParent = InstancesParent != null ? InstancesParent : transform;
            List<LibretroInstance> libretroInstances = GetLibretroInstances(instancesParent);
            if (libretroInstances.Count == 0)
                return;

            ConfigFileContentList contentList = new ConfigFileContentList(libretroInstances.Count);
            for (int i = 0; i < libretroInstances.Count; ++i)
                contentList.Entries[i] = new ConfigFileContent(libretroInstances[i]);

            string json = JsonUtility.ToJson(contentList, true);
            if (string.IsNullOrEmpty(json))
                return;

            File.WriteAllText(_configFilePath, json);
        }

        private static List<LibretroInstance> GetLibretroInstances(Transform parent)
        {
            List<LibretroInstance> libretroInstances = new List<LibretroInstance>();

            if (parent.TryGetComponent(out LibretroInstance libretroInstance))
                libretroInstances.Add(libretroInstance);

            foreach (Transform t0 in parent)
            {
                if (t0.TryGetComponent(out libretroInstance))
                    libretroInstances.Add(libretroInstance);

                foreach (Transform t1 in t0)
                {
                    if (t1.TryGetComponent(out libretroInstance))
                        libretroInstances.Add(libretroInstance);

                    libretroInstances.AddRange(GetLibretroInstances(t1));
                }
            }

            return libretroInstances;
        }
    }
}
