using NAudio.Wave;
using SK.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using static SK.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        #region Events
        public delegate void OnCoreStartedDelegate(LibretroCore core);
        public static event OnCoreStartedDelegate OnCoreStartedEvent;
        public delegate void OnCoreStoppedDelegate(LibretroCore core);
        public static event OnCoreStoppedDelegate OnCoreStoppedEvent;
        public delegate void OnGameStartedDelegate(LibretroGame game);
        public static event OnGameStartedDelegate OnGameStartedEvent;
        public delegate void OnGameStoppedDelegate(LibretroGame game);
        public static event OnGameStoppedDelegate OnGameStoppedEvent;
        #endregion

        public LibretroCore Core { get; private set; }
        public LibretroGame Game { get; private set; }

        public static readonly string WrapperDirectory = $"{Application.streamingAssetsPath}/.libretro";
        public static readonly string CoresDirectory   = $"{WrapperDirectory}/cores";
        public static readonly string SystemDirectory  = $"{WrapperDirectory}/system";
        public static readonly string CoreOptionsFile  = $"{WrapperDirectory}/core_options.json";

        private CoreOptionsList _coreOptionsList;

        private retro_environment_t _environmentCallback;
        private retro_video_refresh_t _videoRefreshCallback;
        private retro_audio_sample_t _audioSampleCallback;
        private retro_audio_sample_batch_t _audioSampleBatchCallback;
        private retro_input_poll_t _inputPollCallback;
        private retro_input_state_t _inputStateCallback;
        private retro_log_printf_t _logPrintfCallback;

        private retro_system_info _systemInfo;
        private retro_system_av_info _systemAVInfo;
        private retro_game_info _gameInfo;
        private retro_pixel_format _pixelFormat;

        private WaveOutEvent _audioDevice;
        private WaveFormat _audioFormat;
        private BufferedWaveProvider _bufferedWaveProvider;

        public unsafe void StartCore(string corePath)
        {
            try
            {
                _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
                if (_coreOptionsList == null)
                {
                    _coreOptionsList = new CoreOptionsList();
                }

                Core = new LibretroCore(corePath);
                SetCallbacks();

                _systemInfo = new retro_system_info();
                Core.retro_get_system_info(ref _systemInfo);
                Core.CoreName = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_name);
                Core.CoreVersion = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_version);
                Core.ValidExtensions = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.valid_extensions).Split('|');
                Core.RequiresFullPath = _systemInfo.need_fullpath;
                Core.BlockExtraction = _systemInfo.block_extract;

                Core.retro_set_environment(_environmentCallback);
                Core.retro_init();
                Core.retro_set_video_refresh(_videoRefreshCallback);
                Core.retro_set_audio_sample(_audioSampleCallback);
                Core.retro_set_audio_sample_batch(_audioSampleBatchCallback);
                Core.retro_set_input_poll(_inputPollCallback);
                Core.retro_set_input_state(_inputStateCallback);

                Core.Initialized = true;

                OnCoreStartedEvent?.Invoke(Core);
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "Libretro.Wrapper.StartCore");
            }
        }

        public void StartGame(string gamePath)
        {
            if (Core.Initialized)
            {
                try
                {
                    Game = new LibretroGame(gamePath);

                    GetGameInfo(gamePath);
                    if (Core.retro_load_game(ref _gameInfo))
                    {
                        _systemAVInfo = new retro_system_av_info();
                        Core.retro_get_system_av_info(ref _systemAVInfo);

                        Game.BaseWidth = Convert.ToInt32(_systemAVInfo.geometry.base_width);
                        Game.BaseHeight = Convert.ToInt32(_systemAVInfo.geometry.base_height);
                        Game.MaxWidth = Convert.ToInt32(_systemAVInfo.geometry.max_width);
                        Game.MaxHeight = Convert.ToInt32(_systemAVInfo.geometry.max_height);
                        Game.AspectRatio = _systemAVInfo.geometry.aspect_ratio;
                        Game.Fps = Convert.ToSingle(_systemAVInfo.timing.fps);
                        Game.SampleRate = Convert.ToInt32(_systemAVInfo.timing.sample_rate);

                        InitAudioDevice(Game.SampleRate, 2);

                        Game.Running = true;

                        OnGameStartedEvent?.Invoke(Game);
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e.Message, "Libretro.Wrapper.StartGame");
                }
            }
            else
            {
                Log.Error("Core not initialized!", "Libretro.Wrapper.StartGame");
            }
        }

        public void StopGame()
        {
            if (Game == null || !Game.Running || Core == null || !Core.Initialized)
            {
                return;
            }

            OnGameStoppedEvent?.Invoke(Game);

            try
            {
                if (Game.Running)
                {
                    Core.retro_unload_game();
                }

                if (Game.internalData != null)
                {
                    Marshal.FreeHGlobal(Game.internalData);
                }

                _audioDevice?.Stop();
                _audioDevice?.Dispose();

                OnCoreStoppedEvent?.Invoke(Core);

                Core.DeInit();
                Core = null;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
            }
        }

        //int numFrames;

        public void Update()
        {
            if (Game == null || !Game.Running || Core == null || !Core.Initialized)
            {
                return;
            }

            //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            Core.retro_run();
            //sw.Stop();
            //if (numFrames++ >= 120)
            //{
            //    Log.Info($"{sw.Elapsed.TotalMilliseconds}ms");
            //    numFrames = 0;
            //}
        }

        private void InitAudioDevice(int rate, int channels)
        {
            try
            {
                _audioDevice = new WaveOutEvent
                {
                    DesiredLatency = 200
                };
                _audioFormat = WaveFormat.CreateIeeeFloatWaveFormat(rate, channels);
                _bufferedWaveProvider = new BufferedWaveProvider(_audioFormat)
                {
                    DiscardOnBufferOverflow = true,
                    BufferLength = 65536
                };
                _audioDevice.Init(_bufferedWaveProvider);
                _audioDevice.Play();
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "Libretro.Wrapper.InitAudioDevice");
            }
        }

        public string GetGamePath(string directory, string gameName)
        {
            foreach (string extension in Core.ValidExtensions)
            {
                string filePath = FileSystem.GetAbsolutePath($"{directory}/{gameName}.{extension}");
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        private unsafe void GetGameInfo(string gamePath)
        {
            try
            {
                using (FileStream stream = new FileStream(gamePath, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    _ = stream.Read(data, 0, (int)stream.Length);
                    Game.internalData = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf<byte>());
                    Marshal.Copy(data, 0, Game.internalData, data.Length);
                    _gameInfo = new retro_game_info
                    {
                        path = StringToChars(gamePath),
                        size = Convert.ToUInt32(data.Length),
                        data = Game.internalData.ToPointer()
                    };
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "Libretro.Wrapper.GetGameInfo");
            }
        }

        private unsafe void SetCallbacks()
        {
            _environmentCallback = RetroEnvironmentCallback;
            _videoRefreshCallback = RetroVideoRefreshCallback;
            _audioSampleCallback = RetroAudioSampleCallback;
            _audioSampleBatchCallback = RetroAudioSampleBatchCallback;
            _inputPollCallback = RetroInputPollCallback;
            _inputStateCallback = RetroInputStateCallback;
            _logPrintfCallback = RetroLogPrintf;
        }
    }
}
