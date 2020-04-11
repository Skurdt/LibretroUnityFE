using SK.Libretro.Utilities;
using System.Linq;
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
        public static readonly string ExtractDirectory = $"{TempDirectory}/extracted";
        public static readonly string CoreOptionsFile  = $"{WrapperDirectory}/core_options.json";

        public bool OptionCropOverscan = true;

        public IGraphicsProcessor GraphicsProcessor;
        public IAudioProcessor AudioProcessor;
        public IInputProcessor InputProcessor;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private CoreOptionsList _coreOptionsList;

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
            AudioProcessor.Init((int)Game.SystemAVInfo.timing.sample_rate);
        }

        public void DeactivateAudio()
        {
            AudioProcessor.DeInit();
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
            _coreOptionsList.Cores = _coreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
            for (int i = 0; i < _coreOptionsList.Cores.Count; i++)
            {
                _coreOptionsList.Cores[i].Options.Sort();
            }
            _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
        }
    }
}
