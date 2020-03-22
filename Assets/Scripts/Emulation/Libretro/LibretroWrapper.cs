using SK.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        public class CoreStartedInfo
        {
            public int ApiVersion;
            public string CoreName;
            public string CoreVersion;
            public string ValidExtensions;
            public bool RequiresFullPath;
            public bool BlockExtraction;
        }

        public class GameStartedInfo
        {
            public int BaseWidth;
            public int BaseHeight;
            public int MaxWidth;
            public int MaxHeight;
            public float AspectRatio;
            public float TargetFps;
            public int SampleRate;
        }

        public delegate void OnCoreStartedDelegate(CoreStartedInfo info);
        public delegate void OnGameStartedDelegate(GameStartedInfo info);
        public static event OnCoreStartedDelegate OnCoreStartedEvent;
        public static event OnGameStartedDelegate OnGameStartedEvent;
        public static event Action OnGameStoppedEvent;

        public unsafe string SystemInfoName { get => Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_name); }
        public unsafe string SystemInfoVersion { get => Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_version); }
        public unsafe string SystemInfoExtensions { get => Marshal.PtrToStringAnsi((IntPtr)_systemInfo.valid_extensions); }
        public RetroSystemAVInfo SystemAVInfo { get => _systemAVInfo; }

        private Core _core;
        private string _systemDirectory;
        private bool _gameRunning = false;

        private readonly LibretroAudioSource _audioSource;
        private readonly Dictionary<string, string> _environmentSettings = new Dictionary<string, string>();
        private RetroSystemInfo _systemInfo;
        private RetroSystemAVInfo _systemAVInfo;
        private RetroPixelFormat _pixelFormat;

        public Wrapper(LibretroAudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public bool StartCore(string corePath)
        {
            try
            {
                _systemDirectory = FileSystem.GetAlsolutePath($"{Path.GetDirectoryName(corePath)}/system");

                _core = new Core(corePath);
                SetCallbacks();
                GetSystemInfo();
                _core.RetroSetEnvironment(_environmentCallback);
                _core.RetroInit();
                _core.RetroSetVideoRefresh(_videoRefreshCallback);
                _core.RetroSetAudioSample(_audioSampleCallback);
                _core.RetroSetAudioSampleBatch(_audioSampleBatchCallback);
                _core.RetroSetInputPoll(_inputPollCallback);
                _core.RetroSetInputState(_inputStateCallback);

                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "Libretro.Wrapper.RunCore");
            }

            return false;
        }

        public void StartGame(string romPath)
        {
            try
            {
                _gameRunning = LoadGame(romPath);
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "Libretro.Wrapper.RunGame");
            }
        }

        public void StopGame()
        {
            OnGameStoppedEvent?.Invoke();

            try
            {
                if (_gameRunning)
                {
                    _core.RetroUnloadGame();
                }
                if (_audioSource != null)
                {
                    _audioSource.StopAudio();
                }
                _core.RetroDeinit();
                _core.DeInit();
                _core = null;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
            }
        }

        public void Update()
        {
            _core?.RetroRun();
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

        private unsafe void GetSystemInfo()
        {
            _systemInfo = new RetroSystemInfo();
            _core.RetroGetSystemInfo(ref _systemInfo);

            CoreStartedInfo info = new CoreStartedInfo
            {
                ApiVersion = _core.RetroApiVersion(),
                CoreName = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_name),
                CoreVersion = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.library_version),
                ValidExtensions = Marshal.PtrToStringAnsi((IntPtr)_systemInfo.valid_extensions),
                RequiresFullPath = _systemInfo.need_fullpath,
                BlockExtraction = _systemInfo.block_extract
            };
            OnCoreStartedEvent?.Invoke(info);
        }

        private bool LoadGame(string gamePath)
        {
            bool result = false;

            RetroGameInfo gameInfo = GetGameInfo(gamePath);
            if (_core.RetroLoadGame(ref gameInfo))
            {
                _systemAVInfo = new RetroSystemAVInfo();
                _core.RetroGetSystemAVInfo(ref _systemAVInfo);

                SetAudioSampleRate((int)_systemAVInfo.timing.sample_rate);
                _audioSource.StartAudio();

                GameStartedInfo info = new GameStartedInfo
                {
                    BaseWidth = Convert.ToInt32(_systemAVInfo.geometry.base_width),
                    BaseHeight = Convert.ToInt32(_systemAVInfo.geometry.base_height),
                    MaxWidth = Convert.ToInt32(_systemAVInfo.geometry.max_width),
                    MaxHeight = Convert.ToInt32(_systemAVInfo.geometry.max_height),
                    AspectRatio = _systemAVInfo.geometry.aspect_ratio,
                    TargetFps = (float)_systemAVInfo.timing.fps,
                    SampleRate = Convert.ToInt32(_systemAVInfo.timing.sample_rate)
                };
                OnGameStartedEvent?.Invoke(info);

                result = true;
            }

            return result;
        }

        private static unsafe RetroGameInfo GetGameInfo(string gamePath)
        {
            try
            {
                using (FileStream stream = new FileStream(gamePath, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    _ = stream.Read(data, 0, (int)stream.Length);
                    IntPtr arrayPointer = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf<byte>());
                    Marshal.Copy(data, 0, arrayPointer, data.Length);
                    return new RetroGameInfo
                    {
                        path = StringToChar(gamePath),
                        size = (uint)data.Length,
                        data = arrayPointer.ToPointer()
                    };
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "GetGameInfo");
                return default;
            }
        }

        public string GetGamePath(string directory, string gameName)
        {
            string result = null;

            string[] extensions = SystemInfoExtensions.Split('|');
            foreach (string extension in extensions)
            {
                string filePath = FileSystem.GetAlsolutePath($"{directory}/{gameName}.{extension}");
                if (File.Exists(filePath))
                {
                    result = filePath;
                    break;
                }
            }

            return result;
        }
    }
}
