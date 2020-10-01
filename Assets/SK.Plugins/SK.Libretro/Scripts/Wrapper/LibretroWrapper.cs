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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static SK.Libretro.LibretroDelegates;
using static SK.Libretro.LibretroEnums;
using static SK.Libretro.LibretroStructs;

namespace SK.Libretro
{
    public sealed class LibretroWrapper
    {
        public enum Language
        {
            English             = 0,
            Japanese            = 1,
            French              = 2,
            Spanish             = 3,
            German              = 4,
            Italian             = 5,
            Dutch               = 6,
            Portuguese_brazil   = 7,
            Portuguese_portugal = 8,
            Russian             = 9,
            Korean              = 10,
            Chinese_traditional = 11,
            Chinese_simplified  = 12,
            Esperanto           = 13,
            Polish              = 14,
            Vietnamese          = 15,
            Arabic              = 16,
            Greek               = 17,
            Turkish             = 18,
            Slovak              = 19,
            Persian             = 20,
            Hebrew              = 21,
            Asturian            = 22
        }

        public bool OptionCropOverscan
        {
            get => _optionCropOverscan;
            set
            {
                if (_optionCropOverscan != value)
                {
                    _optionCropOverscan         = value;
                    Environment.UpdateVariables = true;
                }
            }
        }

        public string OptionUserName
        {
            get => _optionUserName;
            set
            {
                if (!_optionUserName.Equals(value, StringComparison.Ordinal))
                {
                    _optionUserName             = value;
                    Environment.UpdateVariables = true;
                }
            }
        }

        public Language OptionLanguage
        {
            get => _optionLanguage;
            set
            {
                if (_optionLanguage != value)
                {
                    _optionLanguage             = value;
                    Environment.UpdateVariables = true;
                }
            }
        }

        public readonly LibretroVideo Video;
        public readonly LibretroAudio Audio;
        public readonly LibretroInput Input;

        public readonly LibretroCore Core;
        public readonly LibretroGame Game;

        internal static readonly retro_log_level LogLevel = retro_log_level.RETRO_LOG_WARN;

        internal static string MainDirectory;
        internal static string CoresDirectory;
        internal static string SystemDirectory;
        internal static string CoreAssetsDirectory;
        internal static string SavesDirectory;
        internal static string TempDirectory;
        internal static string CoreOptionsFile;

        internal static LibretroCoreOptionsList CoreOptionsList;

        internal readonly LibretroTargetPlatform TargetPlatform;

        internal readonly LibretroEnvironment Environment;

        private string _optionUserName   = "LibretroUnityFE's Awesome User";
        private Language _optionLanguage = Language.English;
        private bool _optionCropOverscan = true;

        #region GC pinned delegates
        internal readonly retro_environment_t EnvironmentCallback;
        internal readonly retro_video_refresh_t VideoRefreshCallback;
        internal readonly retro_audio_sample_t AudioSampleCallback;
        internal readonly retro_audio_sample_batch_t AudioSampleBatchCallback;
        internal readonly retro_input_poll_t InputPollCallback;
        internal readonly retro_input_state_t InputStateCallback;
        internal readonly retro_log_printf_t LogPrintfCallback;
        #endregion

        internal retro_frame_time_callback FrameTimeInterface;
        internal retro_frame_time_callback_t FrameTimeInterfaceCallback = null;

        public retro_hw_render_callback HwRenderInterface;

        private long _frameTimeLast = 0;

        private LibretroPlugin.InteropInterface _interopInterface;

        public unsafe LibretroWrapper(LibretroTargetPlatform targetPlatform, string baseDirectory = null)
        {
            TargetPlatform = targetPlatform;

            if (MainDirectory == null)
            {
                MainDirectory       = !string.IsNullOrEmpty(baseDirectory) ? baseDirectory : "libretro~";
                CoresDirectory      = $"{MainDirectory}/cores";
                SystemDirectory     = $"{MainDirectory}/system";
                CoreAssetsDirectory = $"{MainDirectory}/core_assets";
                SavesDirectory      = $"{MainDirectory}/saves";
                TempDirectory       = $"{MainDirectory}/temp";
                CoreOptionsFile     = $"{MainDirectory}/core_options.json";

                string dir = FileSystem.GetAbsolutePath(MainDirectory);
                if (!Directory.Exists(dir))
                {
                    _ = Directory.CreateDirectory(dir);
                }

                dir = FileSystem.GetAbsolutePath(CoresDirectory);
                if (!Directory.Exists(dir))
                {
                    _ = Directory.CreateDirectory(dir);
                }

                dir = Path.GetFullPath(TempDirectory);
                if (Directory.Exists(dir))
                {
                    string[] fileNames = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                    foreach (string fileName in fileNames)
                    {
                        _ = FileSystem.DeleteFile(fileName);
                    }
                }
            }

            Core = new LibretroCore(this);
            Game = new LibretroGame();

            Environment = new LibretroEnvironment(this);
            Video       = new LibretroVideo(Core, Game);
            Audio       = new LibretroAudio();
            Input       = new LibretroInput();

            EnvironmentCallback       = Environment.Callback;
            VideoRefreshCallback      = Video.Callback;
            AudioSampleCallback       = Audio.SampleCallback;
            AudioSampleBatchCallback  = Audio.SampleBatchCallback;
            InputPollCallback         = Input.PollCallback;
            InputStateCallback        = Input.StateCallback;

            LogPrintfCallback = LibretroLog.RetroLogPrintf;
        }

        public bool StartGame(string coreName, string gameDirectory, string gameName)
        {
            LoadCoreOptionsFile();

            if (!Core.Start(coreName))
            {
                return false;
            }

            if (!Game.Start(Core, gameDirectory, gameName))
            {
                return false;
            }

            if (Core.HwAccelerated)
            {
                _interopInterface = new LibretroPlugin.InteropInterface
                {
                    context_reset = HwRenderInterface.context_reset,
                    context_destroy = HwRenderInterface.context_destroy,
                    retro_run = Marshal.GetFunctionPointerForDelegate(Core.retro_run)
                };
                LibretroPlugin.SetupInteropInterface(ref _interopInterface);
            }

            return true;
        }

        public void StopGame()
        {
            Audio.Processor?.DeInit();

            Game.Stop();
            Core.Stop();
        }

        public void FrameTimeUpdate()
        {
            if (FrameTimeInterface.callback != IntPtr.Zero)
            {
                long current = System.Diagnostics.Stopwatch.GetTimestamp();
                long delta   = current - _frameTimeLast;

                if (_frameTimeLast <= 0)
                {
                    delta = FrameTimeInterface.reference;
                }
                _frameTimeLast = current;
                FrameTimeInterfaceCallback(delta * 1000);
            }
        }

        public void Update()
        {
            if (!Game.Running || !Core.Initialized)
            {
                return;
            }

            if (!Core.HwAccelerated)
            {
                Core.retro_run();
            }
        }

        public void ActivateGraphics(IGraphicsProcessor graphicsProcessor) => Video.Processor = graphicsProcessor;

        public void DeactivateGraphics() => Video.Processor = null;

        public void ActivateAudio(IAudioProcessor audioProcessor)
        {
            Audio.Processor = audioProcessor;
            Audio.Processor?.Init((int)Game.SystemAVInfo.timing.sample_rate);
        }

        public void DeactivateAudio()
        {
            Audio.Processor?.DeInit();
            Audio.Processor = null;
        }

        public void ActivateInput(IInputProcessor inputProcessor) => Input.Processor = inputProcessor;

        public void DeactivateInput() => Input.Processor = null;

        internal static void LoadCoreOptionsFile()
        {
            CoreOptionsList = FileSystem.DeserializeFromJson<LibretroCoreOptionsList>(CoreOptionsFile);
            if (CoreOptionsList == null)
            {
                CoreOptionsList = new LibretroCoreOptionsList();
            }
        }

        internal static void SaveCoreOptionsFile()
        {
            if (CoreOptionsList == null || CoreOptionsList.Cores.Count == 0)
            {
                return;
            }

            CoreOptionsList.Cores = CoreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
            for (int i = 0; i < CoreOptionsList.Cores.Count; ++i)
            {
                CoreOptionsList.Cores[i].Options.Sort();
            }
            _ = FileSystem.SerializeToJson(CoreOptionsList, CoreOptionsFile);
        }
    }
}
