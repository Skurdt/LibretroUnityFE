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
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public bool OptionCropOverscan
        {
            get => _optionCropOverscan;
            set
            {
                if (_optionCropOverscan != value)
                {
                    _optionCropOverscan = value;
                    _dirtyVariables     = true;
                }
            }
        }

        public bool ForceQuit { get; private set; } = false;

        public IGraphicsProcessor GraphicsProcessor { get; private set; }
        public IAudioProcessor AudioProcessor { get; private set; }
        public IInputProcessor InputProcessor { get; private set; }

        public readonly TargetPlatform TargetPlatform;

        public static string WrapperDirectory;
        public static string CoresDirectory;
        public static string SystemDirectory;
        public static string CoreAssetsDirectory;
        public static string SavesDirectory;
        public static string TempDirectory;
        public static string CoreOptionsFile;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private readonly List<IntPtr> _unsafeStrings = new List<IntPtr>();

        private CoreOptionsList _coreOptionsList;
        private bool _optionCropOverscan = true;
        private bool _dirtyVariables     = true;

        public Wrapper(TargetPlatform targetPlatform, string baseDirectory = null)
        {
            TargetPlatform = targetPlatform;

            if (WrapperDirectory == null)
            {
                WrapperDirectory    = !string.IsNullOrEmpty(baseDirectory) ? baseDirectory : "libretro~";
                CoresDirectory      = $"{WrapperDirectory}/cores";
                SystemDirectory     = $"{WrapperDirectory}/system";
                CoreAssetsDirectory = $"{WrapperDirectory}/core_assets";
                SavesDirectory      = $"{WrapperDirectory}/saves";
                TempDirectory       = $"{WrapperDirectory}/temp";
                CoreOptionsFile     = $"{WrapperDirectory}/core_options.json";
            }

            string wrapperDirectory = FileSystem.GetAbsolutePath(WrapperDirectory);
            if (!Directory.Exists(wrapperDirectory))
            {
                _ = Directory.CreateDirectory(wrapperDirectory);
            }

            string tempDirectory = FileSystem.GetAbsolutePath(TempDirectory);
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        public bool StartGame(string coreName, string gameDirectory, string gameName)
        {
            LoadCoreOptionsFile();

            if (!Core.Start(this, coreName))
            {
                return false;
            }

            if (!Game.Start(Core, gameDirectory, gameName))
            {
                return false;
            }

            return true;
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

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public void Update()
        {
            if (ForceQuit || !Game.Running || !Core.Initialized)
            {
                return;
            }

            // FIXME(Tom):
            // An AccessViolationException get thrown by the core when files (roms, bios, etc...) are missing and probably for other various reasons...
            // In a normal C# project, this get captured here (when using the attributes) and errors can be displayed properly.
            // Unity simply crashes here but we only know about the missing things after a call to retro_run...
            try
            {
                Core.retro_run();
            }
            catch (AccessViolationException e)
            {
                Log.Exception(e);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public void ActivateGraphics(IGraphicsProcessor graphicsProcessor) => GraphicsProcessor = graphicsProcessor;

        public void DeactivateGraphics() => GraphicsProcessor = null;

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

        public void ActivateInput(IInputProcessor inputProcessor) => InputProcessor = inputProcessor;

        public void DeactivateInput() => InputProcessor = null;

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
            if (_coreOptionsList == null || _coreOptionsList.Cores.Count == 0)
            {
                return;
            }

            _coreOptionsList.Cores = _coreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
            for (int i = 0; i < _coreOptionsList.Cores.Count; ++i)
            {
                _coreOptionsList.Cores[i].Options.Sort();
            }
            _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
        }
    }
}
