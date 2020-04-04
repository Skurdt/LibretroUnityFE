using SK.Utilities;
using System.IO;
using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        #region Events
        public delegate void OnCoreStartedDelegate(LibretroCore core);
        public event OnCoreStartedDelegate OnCoreStartedEvent;
        public delegate void OnCoreStoppedDelegate(LibretroCore core);
        public event OnCoreStoppedDelegate OnCoreStoppedEvent;
        public delegate void OnGameStartedDelegate(LibretroGame game);
        public event OnGameStartedDelegate OnGameStartedEvent;
        public delegate void OnGameStoppedDelegate(LibretroGame game);
        public event OnGameStoppedDelegate OnGameStoppedEvent;
        #endregion

        public IAudioProcessor AudioProcessor;
        public IInputProcessor InputProcessor;

        public static readonly string WrapperDirectory = $"{Application.streamingAssetsPath}/.libretro";
        public static readonly string CoresDirectory   = $"{WrapperDirectory}/cores";
        public static readonly string SystemDirectory  = $"{WrapperDirectory}/system";
        public static readonly string SavesDirectory   = $"{WrapperDirectory}/saves";
        public static readonly string TempDirectory    = $"{WrapperDirectory}/temp";
        public static readonly string CoreOptionsFile  = $"{WrapperDirectory}/core_options.json";

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private CoreOptionsList _coreOptionsList;

        public unsafe bool StartCore(string corePath)
        {
            bool result = false;

            _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
            if (_coreOptionsList == null)
            {
                _coreOptionsList = new CoreOptionsList();
            }

            if (Core.Start(this, corePath))
            {
                OnCoreStartedEvent?.Invoke(Core);
                result = true;
            }

            return result;
        }

        public bool StartGame(string gamePath)
        {
            bool result = false;

            if (Core.Initialized)
            {
                if (Game.Start(Core, gamePath))
                {
                    AudioProcessor?.Init(Game.SampleRate);
                    OnGameStartedEvent?.Invoke(Game);
                    result = true;
                }
            }
            else
            {
                Log.Error("Core not initialized!", "Libretro.Wrapper.StartGame");
            }

            return result;
        }

        public void StopGame()
        {
            AudioProcessor?.DeInit();

            Game.Stop();
            OnGameStoppedEvent?.Invoke(Game);

            OnCoreStoppedEvent?.Invoke(Core);
            Core.Stop();
        }

        public void Update()
        {
            if (!Game.Running || !Core.Initialized)
            {
                return;
            }

            Core.retro_run();
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
    }
}
