using SK.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    [Serializable]
    public class Game
    {
        public string Core;
        public string Directory;
        public string Name;
    }

    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        [SerializeField] private Game _game;

        public Libretro.Wrapper Wrapper { get; private set; } = new Libretro.Wrapper();

        private Renderer _rendererComponent = null;
        private Material _originalMaterial = null;

        private void Awake()
        {
            if (transform.childCount > 0)
            {
                Transform modelTransform = transform.GetChild(0);
                if (modelTransform != null && modelTransform.childCount > 1)
                {
                    Transform screenTransform = modelTransform.GetChild(1);
                    if (screenTransform.TryGetComponent(out _rendererComponent))
                    {
                        _originalMaterial = _rendererComponent.sharedMaterial;
                    }
                }
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(_game.Name))
            {
                StartGame();
            }
        }

        private void OnEnable()
        {
            Wrapper.OnCoreStartedEvent += CoreStartedCallback;
            Wrapper.OnCoreStoppedEvent += CoreStoppedCallback;
            Wrapper.OnGameStartedEvent += GameStartedCallback;
            Wrapper.OnGameStoppedEvent += GameStoppedCallback;
        }

        private void OnDisable()
        {
            if (Wrapper != null)
            {
                Wrapper.OnCoreStartedEvent -= CoreStartedCallback;
                Wrapper.OnCoreStoppedEvent -= CoreStoppedCallback;
                Wrapper.OnGameStartedEvent -= GameStartedCallback;
                Wrapper.OnGameStoppedEvent -= GameStoppedCallback;
            }
        }

        private void OnDestroy()
        {
            StopGame();
        }

        public void ActivateInput()
        {
            Wrapper.InputProcessor = FindObjectOfType<PlayerInputManager>().GetComponent<Libretro.IInputProcessor>();
        }

        public void DeactivateInput()
        {
            Wrapper.InputProcessor = null;
        }

        public void StartGame()
        {
            if (!Wrapper.Game.Running)
            {
                Wrapper.AudioProcessor = GetComponentInChildren<Libretro.IAudioProcessor>() ?? new AudioProcessorExternal();

                string corePath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.WrapperDirectory}/cores/{_game.Core}_libretro.dll");
                if (FileSystem.FileExists(corePath))
                {
                    string instancePath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.TempDirectory}/{Path.GetFileName(corePath).Replace("_libretro", $"{Guid.NewGuid()}")}");
                    File.Copy(corePath, instancePath);

                    if (Wrapper.StartCore(instancePath))
                    {
                        if (!string.IsNullOrEmpty(_game.Name))
                        {
                            string gamePath = Wrapper.GetGamePath(_game.Directory, _game.Name);
                            if (gamePath == null)
                            {
                                // Try Zip archive
                                string archivePath = FileSystem.GetAbsolutePath($"{_game.Directory}/{_game.Name}.zip");
                                if (File.Exists(archivePath))
                                {
                                    string extractPath = FileSystem.GetAbsolutePath(Libretro.Wrapper.TempDirectory);
                                    if (Directory.Exists(extractPath))
                                    {
                                        Directory.Delete(extractPath, true);
                                    }
                                    System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
                                    gamePath = Wrapper.GetGamePath(extractPath, _game.Name);
                                }
                            }

                            if (gamePath != null)
                            {
                                if (Wrapper.StartGame(gamePath))
                                {
                                    InvokeRepeating("LibretroRunLoop", 0f, 1f / Wrapper.Game.Fps);
                                }
                                else
                                {
                                    Log.Error($"Game '{_game.Name}' failed to start on core '{_game.Core}'.", "GameModelSetup.StartGame");
                                }
                            }
                            else
                            {
                                Log.Error($"Game '{_game.Name}' at path '{_game.Directory}' not found.", "GameModelSetup.StartGame");
                            }
                        }
                        else
                        {
                            Log.Warning($"Game not set, running '{_game.Core}' core only.", "GameModelSetup.StartGame");
                        }
                    }
                    else
                    {
                        Log.Warning($"Failed to start core '{_game.Core}'.", "GameModelSetup.StartGame");
                    }
                }
                else
                {
                    Log.Error($"Core '{_game.Core}' at path '{corePath}' not found.", "GameModelSetup.StartGame");
                }
            }
        }

        public void StopGame()
        {
            CancelInvoke();
            Wrapper?.StopGame();
            Wrapper = null;
        }

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by InvokeRepeating")]
        private void LibretroRunLoop()
        {
            Wrapper.Update();
            if (Wrapper.Texture != null)
            {
                _rendererComponent.material.mainTexture = Wrapper.Texture;
                _rendererComponent.material.SetTexture("_EmissionMap", Wrapper.Texture);
            }
        }

        private void CoreStartedCallback(Libretro.LibretroCore core)
        {
        }

        private void CoreStoppedCallback(Libretro.LibretroCore core)
        {
        }

        private void GameStartedCallback(Libretro.LibretroGame game)
        {
            _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
            _rendererComponent.material.EnableKeyword("_EMISSION");
            _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            _rendererComponent.material.SetColor("_EmissionColor", Color.white);
        }

        private void GameStoppedCallback(Libretro.LibretroGame game)
        {
            _rendererComponent.material = _originalMaterial;
        }
    }
}
