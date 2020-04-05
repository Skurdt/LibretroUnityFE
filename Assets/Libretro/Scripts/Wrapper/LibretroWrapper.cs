using SK.Libretro.Utilities;
using System.IO;
using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public static readonly string WrapperDirectory = $"{Application.streamingAssetsPath}/.libretro";
        public static readonly string CoresDirectory   = $"{WrapperDirectory}/cores";
        public static readonly string SystemDirectory  = $"{WrapperDirectory}/system";
        public static readonly string SavesDirectory   = $"{WrapperDirectory}/saves";
        public static readonly string TempDirectory    = $"{WrapperDirectory}/temp";
        public static readonly string CoreOptionsFile  = $"{WrapperDirectory}/core_options.json";

        public IGraphicsProcessor GraphicsProcessor;
        public IAudioProcessor AudioProcessor;
        public IInputProcessor InputProcessor;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private CoreOptionsList _coreOptionsList;

        public unsafe bool StartCore(string corePath)
        {
            _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
            if (_coreOptionsList == null)
            {
                _coreOptionsList = new CoreOptionsList();
            }

            return Core.Start(this, corePath);
        }

        public bool StartGame(string gamePath)
        {
            bool result = false;

            if (Core.Initialized)
            {
                if (Game.Start(Core, gamePath))
                {
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
