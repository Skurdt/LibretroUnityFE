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
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    [SelectionBase, DisallowMultipleComponent]
    public abstract class GameModelSetup : MonoBehaviour
    {
        [SerializeField] protected Transform _viewer                  = default;
        [SerializeField] protected MainMenuUI _menu                   = default;
        [SerializeField] protected TMP_Text _rewindText               = default;
        [SerializeField] protected TMP_Text _analogToDigitalInputText = default;

        public string CoreName { get; set; }
        public string GameDirectory { get; set; }
        public string GameName { get; set; }
        public bool Running => !(_libretro is null) && _libretro.Running;
        public bool InputEnabled
        {
            get => !(_libretro is null) && _libretro.InputEnabled;
            set
            {
                if (!(_libretro is null))
                    _libretro.InputEnabled = value;
            }
        }
        public bool AnalogToDigitalInput { get; private set; } = false;
        public bool RewindEnabled { get; private set; } = false;

        protected static int _playerLayer     = -1;
        protected static bool _playerLayerSet = false;

        private const string REWIND_ON_STRING                   = "Rewind: On";
        private const string REWIND_OFF_STRING                  = "Rewind: Off";
        private const string ANALOG_TO_DIGITAL_INPUT_ON_STRING  = "Analog To Digital: On";
        private const string ANALOG_TO_DIGITAL_INPUT_OFF_STRING = "Analog To Digital: Off";

        private LibretroBridge _libretro = null;

        private void Awake()
        {
            if (!_playerLayerSet)
            {
                _playerLayer    = LayerMask.NameToLayer("Player");
                _playerLayerSet = true;
            }

            if (_viewer == null)
                _viewer = Camera.main.transform;

            if (_menu == null)
                _menu = FindObjectOfType<MainMenuUI>();

            if (_rewindText != null)
                _rewindText.text = REWIND_OFF_STRING;

            if (_analogToDigitalInputText != null)
                _analogToDigitalInputText.text = ANALOG_TO_DIGITAL_INPUT_OFF_STRING;
        }

        private void Start()
        {
            LoadConfig();

            StartGame();

            if (_libretro is null)
                return;

            OnLateStart();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!(Keyboard.current is null) && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                StopGame();
                ApplicationUtils.ExitApp();
                return;
            }

            if (_libretro is null)
                return;

            OnUpdate();
        }

        private void OnEnable() => Application.focusChanged += OnApplicationFocusChanged;

        private void OnDisable()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            StopGame();
        }

        public void Pause()
        {
            Cursor.lockState = CursorLockMode.None;
            _libretro?.Pause();
        }

        public void Resume()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _libretro?.Resume();
        }

        public bool SaveState(int index, bool saveScreenshot = true) => !(_libretro is null) && _libretro.SaveState(index, saveScreenshot);

        public bool LoadState(int index) => !(_libretro is null) && _libretro.LoadState(index);

        public void Rewind(bool rewind) => _libretro?.Rewind(rewind);

        public void UI_ToggleAnalogToDigitalInput()
        {
            if (_libretro is null)
                return;

            AnalogToDigitalInput = !AnalogToDigitalInput;

            if (_analogToDigitalInputText != null)
                _analogToDigitalInputText.text = AnalogToDigitalInput ? ANALOG_TO_DIGITAL_INPUT_ON_STRING : ANALOG_TO_DIGITAL_INPUT_OFF_STRING;

            _libretro.SetAnalogToDigitalInput(AnalogToDigitalInput);
        }

        public void UI_ToggleRewind()
        {
            if (_libretro is null)
                return;

            RewindEnabled = !RewindEnabled;

            if (_rewindText != null)
                _rewindText.text = RewindEnabled ? REWIND_ON_STRING : REWIND_OFF_STRING;

            _libretro.SetRewindEnabled(RewindEnabled);
        }

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
                Debug.LogWarning("Core not set");
                return;
            }

            LibretroScreenNode screen = GetComponentInChildren<LibretroScreenNode>();
            if (screen == null)
            {
                Debug.LogWarning($"ScreenNode not found, adding ScreenNode component to the same node this script is attached to ({name})");
                screen = gameObject.AddComponent<LibretroScreenNode>();
            }

            if (screen.GetComponent<Renderer>() == null)
            {
                Debug.LogError("Component of type Renderer not found");
                return;
            }

            LibretroBridge.Settings settings = new LibretroBridge.Settings
            {
                AnalogDirectionsToDigital = AnalogToDigitalInput
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
                AnalogDirectionsToDigital = gameModelSetup.AnalogToDigitalInput;
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
            if (game is null)
                return;

            CoreName             = game.Core;
            GameDirectory        = game.Directory;
            GameName             = game.Name;
            AnalogToDigitalInput = game.AnalogDirectionsToDigital;
        }

        [ContextMenu("Save configuration")]
        public void SaveConfig()
        {
            string json = GetJsonConfig();
            if (!string.IsNullOrEmpty(json))
                File.WriteAllText(ConfigFilePath, json);
        }

        protected abstract string ConfigFilePath { get; }

        protected abstract ConfigFileContent LoadJsonConfig(string json);

        protected abstract string GetJsonConfig();
    }
}
