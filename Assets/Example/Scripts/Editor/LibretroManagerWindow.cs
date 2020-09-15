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

using HtmlAgilityPack;
using SK.Libretro.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace SK.Libretro.EditorUtils
{
    public sealed class LibretroManagerWindow : EditorWindow
    {
        [Serializable]
        public sealed class Core
        {
            public string FullName;
            public string DisplayName;
            public string LastModified;
            public bool Available;
            public bool Latest;
        }

        [Serializable]
        public sealed class CoreList
        {
            public List<Core> Cores = new List<Core>();

            public void Add(Core core) => Cores.Add(core);
        }
        static string CurrentPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxEditor:
                    return "linux";
                default:               
                    return "windows";
            }
        }

        private static string _buildBotUrl                = $"https://buildbot.libretro.com/nightly/{CurrentPlatform()}/x86_64/latest/";
        private static readonly string _libretroDirectory = Path.Combine(Application.streamingAssetsPath, "libretro~");
        private static readonly string _coresDirectory    = Path.Combine(_libretroDirectory, "cores");
        private static readonly string _coresStatusFile   = Path.Combine(_libretroDirectory, "cores.json");
        private static readonly Color _orangeColor = new Color(0.6f, 0.6f, 0f, 1f);

        private static CoreList _coreList;
        private static Color _defaultBgColor;
        private Vector2 _scrollPos;

        [MenuItem("Libretro/Manage Cores"), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor")]
        private static void ShowWindow()
        {
            if (!Directory.Exists(_libretroDirectory))
            {
                _ = Directory.CreateDirectory(_libretroDirectory);
            }

            if (!Directory.Exists(_coresDirectory))
            {
                _ = Directory.CreateDirectory(_coresDirectory);
            }

            _defaultBgColor = GUI.backgroundColor;

            _coreList = FileSystem.DeserializeFromJson<CoreList>(_coresStatusFile);
            if (_coreList == null)
            {
                _coreList = new CoreList();
            }

            GetWindow<LibretroManagerWindow>("Core Manager").minSize = new Vector2(286f, 120f);
        }

        private void OnGUI()
        {
            GUILayout.Space(16f);

            if (GUILayout.Button("Check for updates", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
            {
                CheckForUpdates();
            }

            GUILayout.Space(16f);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                foreach (Core core in _coreList.Cores)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(core.DisplayName, GUILayout.Width(180f));

                        string buttonText;
                        if (core.Available && core.Latest)
                        {
                            GUI.backgroundColor = Color.green;
                            buttonText = "OK";
                        }
                        else if (core.Available && !core.Latest)
                        {
                            GUI.backgroundColor = _orangeColor;
                            buttonText = "Update";
                        }
                        else
                        {
                            GUI.backgroundColor = Color.red;
                            buttonText = "Download";
                        }

                        if (GUILayout.Button(new GUIContent(buttonText, null, core.DisplayName), GUILayout.Width(80f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
                            if (!core.Available || !core.Latest)
                            {
                                string url = $"{_buildBotUrl}{core.FullName}";
                                string zipPath = DownloadFile(url);
                                ExtractFile(zipPath);
                                CheckForUpdates();
                            }
                        }

                        GUI.backgroundColor = _defaultBgColor;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        private static void CheckForUpdates()
        {
            HtmlWeb hw                 = new HtmlWeb();
            HtmlDocument doc           = hw.Load(new Uri(_buildBotUrl));
            HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes("//body/div/table/tr");

            foreach (HtmlNode trNode in trNodes)
            {
                HtmlNodeCollection tdNodes = trNode.ChildNodes;
                foreach (HtmlNode node in tdNodes)
                {
                    string fileName = tdNodes[1].InnerText;
                    if (!fileName.Contains("_libretro"))
                    {
                        continue;
                    }

                    string lastModified = tdNodes[2].InnerText;
                    bool available = File.Exists(Path.GetFullPath(Path.Combine(_coresDirectory, fileName.Replace(".zip", string.Empty))));
                    Core found = _coreList.Cores.Find(x => x.FullName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    if (found != null)
                    {
                        found.LastModified = lastModified;
                        found.Available = available;
                        found.Latest = found.LastModified.Equals(lastModified, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {

                        _coreList.Add(new Core
                        {
                            FullName = fileName,
                            DisplayName = fileName.Substring(0,fileName.IndexOf('.')),
                            LastModified = lastModified,
                            Available = available,
                            Latest = true
                        });
                    }
                }
            }

            _coreList.Cores = _coreList.Cores.OrderBy(x => x.DisplayName).ToList();
            _ = FileSystem.SerializeToJson(_coreList, _coresStatusFile);
        }

        private static string DownloadFile(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                string fileName = Path.GetFileName(url);
                string filePath = Path.GetFullPath(Path.Combine(_coresDirectory, fileName));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                webClient.DownloadFile(url, filePath);
                Debug.Log($"Downloaded {Path.GetFileNameWithoutExtension(fileName)}");
                return filePath;
            }
        }

        private static void ExtractFile(string zipPath)
        {
            if (!File.Exists(zipPath))
            {
                return;
            }

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.GetFullPath(Path.Combine(_coresDirectory, entry.FullName));
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }

                        entry.ExtractToFile(destinationPath);
                    }
                }
            }
            finally
            {
                File.Delete(zipPath);
            }

            Debug.Log($"Extracted {Path.GetFileNameWithoutExtension(zipPath)}");
        }
    }
}
