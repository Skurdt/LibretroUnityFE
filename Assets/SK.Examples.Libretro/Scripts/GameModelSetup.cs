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
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    [SelectionBase, DisallowMultipleComponent]
    public abstract class GameModelSetup : MonoBehaviour
    {
        public LibretroInstance LibretroInstance;

        private void Awake()
        {
            if (LibretroInstance == null)
                LibretroInstance = GetComponent<LibretroInstance>();
        }

        private void Start()
        {
            LoadConfig();
            StartGame();
            OnLateStart();
        }

        private void Update()
        {
            if (!(Keyboard.current is null) && Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.xKey.wasPressedThisFrame)
            {
                if (LibretroInstance != null)
                    LibretroInstance.StopContent();
                ApplicationUtils.ExitApp();
                return;
            }

            if (LibretroInstance == null)
                return;

            OnUpdate();
        }

        private void OnEnable() => Application.focusChanged += OnApplicationFocusChanged;

        private void OnDisable()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            if (LibretroInstance != null)
                LibretroInstance.StopContent();
        }

        protected virtual void OnLateStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected void StartGame()
        {
            if (LibretroInstance == null)
                return;

            LibretroInstance.StartContent();
        }

        private void OnApplicationFocusChanged(bool focus)
        {
            if (LibretroInstance == null)
                return;

            if (!focus)
                LibretroInstance.PauseContent();
            else
                LibretroInstance.ResumeContent();
        }

        /***********************************************************************************************************************
         * Config file
         **********************************************************************************************************************/
        [Serializable]
        protected sealed class ConfigFileContent
        {
            public string Core;
            public string Directory;
            public string[] Names;
            public ConfigFileContent(GameModelSetup gameModelSetup)
            {
                if (gameModelSetup.LibretroInstance == null)
                    return;
                Core      = gameModelSetup.LibretroInstance.CoreName;
                Directory = gameModelSetup.LibretroInstance.GamesDirectory;
                Names     = gameModelSetup.LibretroInstance.GameNames;
            }
        }

        [ContextMenu("Load configuration")]
        public void LoadConfig()
        {
            if (LibretroInstance == null)
                return;

            if (!File.Exists(ConfigFilePath))
                return;

            string json = File.ReadAllText(ConfigFilePath);
            if (string.IsNullOrEmpty(json))
                return;

            ConfigFileContent game = LoadJsonConfig(json);
            if (game is null)
                return;

            LibretroInstance.CoreName      = game.Core;
            LibretroInstance.GamesDirectory = game.Directory;
            LibretroInstance.GameNames     = game.Names;
        }

        [ContextMenu("Save configuration")]
        public void SaveConfig()
        {
            if (LibretroInstance == null)
                return;

            string json = GetJsonConfig();
            if (!string.IsNullOrEmpty(json))
                File.WriteAllText(ConfigFilePath, json);
        }

        protected abstract string ConfigFilePath { get; }

        protected abstract ConfigFileContent LoadJsonConfig(string json);

        protected abstract string GetJsonConfig();
    }
}
