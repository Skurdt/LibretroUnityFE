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
using SK.Utilities.Unity;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SK.Examples
{
    [SelectionBase, DisallowMultipleComponent]
    internal abstract class GameModelSetup : MonoBehaviour
    {
        public bool AnalogDirectionsToDigital = false;

        public Toggle AnalogDirectionsToDigitalToggle;
        public Image DownloadProgressImage;
        public Text DownloadProgressText;

        public string CoreName { get; set; }
        public string GameDirectory { get; set; }
        public string GameName { get; set; }
        public bool Running => _libretro != null && _libretro.Running;
        public bool InputEnabled
        {
            get => _libretro != null && _libretro.InputEnabled;
            set
            {
                if (_libretro != null)
                    _libretro.InputEnabled = value;
            }
        }
        public float DownloadProgress { get; private set; } = 0f;

        private LibretroBridge _libretro = null;
        private Transform _viewer        = null;

        private bool _coreReady = false;

        private void Awake()
        {
            _viewer = Camera.main.transform;

            GameObject analogToDigitalToggleGameObject = GameObject.Find("AnalogToDigitalToggle");
            if (analogToDigitalToggleGameObject != null)
            {
                AnalogDirectionsToDigitalToggle = analogToDigitalToggleGameObject.GetComponent<Toggle>();
                if (AnalogDirectionsToDigitalToggle != null)
                    AnalogDirectionsToDigitalToggle.isOn = AnalogDirectionsToDigital;
            }

            if (DownloadProgressImage != null)
                DownloadProgressImage.gameObject.SetActive(false);
            if (DownloadProgressText != null)
                DownloadProgressText.gameObject.SetActive(false);
        }

        private IEnumerator Start()
        {
            LoadConfig();

            TryDownloadCore();

            while (!_coreReady)
                yield return null;

            StartGame();

            if (_libretro != null && _libretro.Running)
                OnLateStart();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                StopGame();
                ApplicationUtils.ExitApp();
                return;
            }

            if (_libretro != null && _libretro.Running)
            {
                OnUpdate();
                _libretro.Update();
            }
        }

        private void OnEnable() => Application.focusChanged += OnApplicationFocusChanged;

        private void OnDisable()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            StopGame();
        }

        public void Pause() => _libretro?.Pause();

        public void Resume() => _libretro?.Resume();

        public bool SaveState(int index, bool saveScreenshot = true) => _libretro != null && _libretro.SaveState(index, saveScreenshot);

        public bool LoadState(int index) => _libretro != null && _libretro.LoadState(index);

        public void Rewind(bool rewind) => _libretro.Rewind(rewind);

        public void UI_SetAnalogToDigitalInput(bool value) => _libretro?.ToggleAnalogToDigitalInput(value);

        protected virtual void OnLateStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected void StartGame()
        {
            if (string.IsNullOrEmpty(CoreName))
            {
                Debug.LogError("Core not set");
                return;
            }

            ScreenNode screen = GetComponentInChildren<ScreenNode>();
            if (screen == null)
            {
                Debug.LogWarning($"ScreenNode not found, adding ScreenNode component to the same node this script is attached to ({name})");
                screen = gameObject.AddComponent<ScreenNode>();
            }

            if (screen.GetComponent<Renderer>() == null)
            {
                Debug.LogError("Component of type Renderer not found");
                return;
            }

            LibretroBridge.Settings settings = new LibretroBridge.Settings
            {
                AnalogDirectionsToDigital = AnalogDirectionsToDigital
            };
            _libretro = new LibretroBridge(screen, _viewer, settings);
            if (!_libretro.Start(CoreName, GameDirectory, GameName))
            {
                StopGame();
                return;
            }
        }

        protected void StopGame()
        {
            _libretro?.Stop();
            _libretro = null;
        }

        private void OnApplicationFocusChanged(bool focus)
        {
            if (!focus)
                _libretro?.Pause();
            else
                _libretro?.Resume();
        }

        /***********************************************************************************************************************
         * Config file
         **********************************************************************************************************************/
        [Serializable]
        protected sealed class ConfigFileContent
        {
            public string Core;
            public string Directory;
            public string Name;
            public bool AnalogDirectionsToDigital;
            public ConfigFileContent(GameModelSetup gameModelSetup)
            {
                Core                      = gameModelSetup.CoreName;
                Directory                 = gameModelSetup.GameDirectory;
                Name                      = gameModelSetup.GameName;
                AnalogDirectionsToDigital = gameModelSetup.AnalogDirectionsToDigital;
            }
        }

        [ContextMenu("Load configuration")]
        public void LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
                return;

            string json = File.ReadAllText(ConfigFilePath);
            if (string.IsNullOrEmpty(json))
                return;

            ConfigFileContent game = LoadJsonConfig(json);
            if (game == null)
                return;

            CoreName                  = game.Core;
            GameDirectory             = game.Directory;
            GameName                  = game.Name;
            AnalogDirectionsToDigital = game.AnalogDirectionsToDigital;

            if (AnalogDirectionsToDigitalToggle != null)
                AnalogDirectionsToDigitalToggle.isOn = AnalogDirectionsToDigital;
        }

        [ContextMenu("Save configuration")]
        private void SaveConfig()
        {
            string json = GetJsonConfig();
            if (!string.IsNullOrEmpty(json))
                File.WriteAllText(ConfigFilePath, json);
        }

        protected abstract string ConfigFilePath { get; }
        protected abstract ConfigFileContent LoadJsonConfig(string json);
        protected abstract string GetJsonConfig();

        /***********************************************************************************************************************
         * Dynamic core download
         **********************************************************************************************************************/
        private void TryDownloadCore()
        {
            string extensionString = GetLibretroBotExtension();
            if (extensionString == null)
                return;

            string coresDirectory = Path.Combine(Application.streamingAssetsPath, "libretro~/cores");
            string corePath       = Path.GetFullPath(Path.Combine(coresDirectory, $"{CoreName}_libretro.{extensionString}"));
            if (File.Exists(corePath))
            {
                _coreReady       = true;
                DownloadProgress = 1f;
                return;
            }

            string platformString = GetLibretroBotPlatform();
            if (platformString == null)
                return;

            string url = GetLibretroBotUrl(platformString, extensionString);
            _ = StartCoroutine(DownloadFile(url, coresDirectory));
        }

        private IEnumerator DownloadFile(string url, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
                _ = Directory.CreateDirectory(destinationDir);

            string filePath = Path.GetFullPath(Path.Combine(destinationDir, Path.GetFileName(url)));
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (UnityWebRequest uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                uwr.downloadHandler = new DownloadHandlerFile(filePath);
                _ = uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                    Debug.LogError(uwr.error);
                else
                {
                    if (DownloadProgressImage != null)
                        DownloadProgressImage.gameObject.SetActive(true);

                    if (DownloadProgressText != null)
                        DownloadProgressText.gameObject.SetActive(true);

                    while (!uwr.downloadHandler.isDone)
                    {
                        DownloadProgress = uwr.downloadProgress;
                        if (DownloadProgressImage != null)
                            DownloadProgressImage.fillAmount = DownloadProgress;
                        if (DownloadProgressText != null)
                            DownloadProgressText.text = $"Downloading core... {DownloadProgress * 100f}%";
                        yield return null;
                    }
                }

                DownloadProgress = 1.0f;
                if (DownloadProgressImage != null)
                {
                    DownloadProgressImage.fillAmount = DownloadProgress;
                    DownloadProgressImage.gameObject.SetActive(false);
                }
                if (DownloadProgressText != null)
                {
                    DownloadProgressText.text = "";
                    DownloadProgressText.gameObject.SetActive(false);
                }

                ExtractFile(filePath, destinationDir);
            }
        }

        private void ExtractFile(string zipPath, string destinationDir)
        {
            try
            {
                if (!File.Exists(zipPath))
                    return;

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.GetFullPath(Path.Combine(destinationDir, entry.FullName));
                        if (File.Exists(destinationPath))
                            File.Delete(destinationPath);
                        entry.ExtractToFile(destinationPath);
                    }
                    _coreReady = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(zipPath);
            }
        }

        private string GetLibretroBotUrl(string platformString, string extensionString)
            => $"https://buildbot.libretro.com/nightly/{platformString}/x86_64/latest/{CoreName}_libretro.{extensionString}.zip";

        private static string GetLibretroBotPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return "linux";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "apple/osx";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "windows";
                default:
                {
                    Debug.LogError($"Invalid/unsupported platform detected: {Application.platform}");
                    return null;
                }
            }
        }

        private static string GetLibretroBotExtension()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return "so";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "dylib";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "dll";
                default:
                {
                    Debug.LogError($"Invalid/unsupported platform detected: {Application.platform}");
                    return null;
                }
            }
        }
    }
}
