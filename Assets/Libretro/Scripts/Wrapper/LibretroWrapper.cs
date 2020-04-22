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

using SK.Libretro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public static string WrapperDirectory    = null;
        public static string CoresDirectory      = null;
        public static string SystemDirectory     = null;
        public static string CoreAssetsDirectory = null;
        public static string SavesDirectory      = null;
        public static string TempDirectory       = null;
        public static string ExtractDirectory    = null;
        public static string CoreOptionsFile     = null;

        public bool OptionCropOverscan = true;

        public TargetPlatform TargetPlatform { get; }

        public IGraphicsProcessor GraphicsProcessor;
        public IAudioProcessor AudioProcessor;
        public IInputProcessor InputProcessor;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private CoreOptionsList _coreOptionsList;

        private readonly List<IntPtr> _unsafeStrings = new List<IntPtr>();

        public Wrapper(TargetPlatform targetPlatform, string baseDirectory = null)
        {
            TargetPlatform = targetPlatform;

            if (WrapperDirectory == null)
            {
                if (baseDirectory == null)
                {
                    baseDirectory = $"{Application.streamingAssetsPath}/libretro~";
                }

                WrapperDirectory    = baseDirectory;
                CoresDirectory      = $"{WrapperDirectory}/cores";
                SystemDirectory     = $"{WrapperDirectory}/system";
                CoreAssetsDirectory = $"{WrapperDirectory}/core_assets";
                SavesDirectory      = $"{WrapperDirectory}/saves";
                TempDirectory       = $"{WrapperDirectory}/temp";
                ExtractDirectory    = $"{TempDirectory}/extracted";
                CoreOptionsFile     = $"{WrapperDirectory}/core_options.json";
            }

            string wrapperDirectoryAbs = FileSystem.GetAbsolutePath(WrapperDirectory);
            if (!Directory.Exists(wrapperDirectoryAbs))
            {
                _ = Directory.CreateDirectory(wrapperDirectoryAbs);
            }
        }

        public bool StartGame(string coreName, string gameDirectory, string gameName)
        {
            bool result = false;

            LoadCoreOptionsFile();

            if (Core.Start(this, coreName))
            {
                if (Game.Start(Core, gameDirectory, gameName))
                {
                    result = true;
                }
            }

            return result;
        }

        public void StopGame()
        {
            AudioProcessor?.DeInit();

            Game.Stop();
            Core.Stop();

            for (int i = 0; i < _unsafeStrings.Count; ++i)
            {
                Marshal.FreeHGlobal(_unsafeStrings[i]);
            }
        }

        public void Update()
        {
            if (!Game.Running || !Core.Initialized)
            {
                return;
            }

            Core.retro_run();
        }

        public void ActivateGraphics(IGraphicsProcessor graphicsProcessor)
        {
            GraphicsProcessor = graphicsProcessor;
        }

        public void DeactivateGraphics()
        {
            GraphicsProcessor = null;
        }

        public void ActivateAudio(IAudioProcessor audioProcessor)
        {
            AudioProcessor = audioProcessor;
            AudioProcessor?.Init((int)Game.SystemAVInfo.timing.sample_rate);
        }

        public void DeactivateAudio()
        {
            AudioProcessor?.DeInit();
            AudioProcessor = null;
        }

        public void ActivateInput(IInputProcessor inputProcessor)
        {
            InputProcessor = inputProcessor;
        }

        public void DeactivateInput()
        {
            InputProcessor = null;
        }

        private void LoadCoreOptionsFile()
        {
            _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
            if (_coreOptionsList == null)
            {
                _coreOptionsList = new CoreOptionsList();
            }
        }

        private void SaveCoreOptionsFile()
        {
            if (_coreOptionsList != null)
            {
                _coreOptionsList.Cores = _coreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
                for (int i = 0; i < _coreOptionsList.Cores.Count; ++i)
                {
                    _coreOptionsList.Cores[i].Options.Sort();
                }
                _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
            }
        }
    }
}
