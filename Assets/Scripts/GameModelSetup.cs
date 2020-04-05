using SK.Libretro.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public Game Game;

        public Libretro.Wrapper Wrapper { get; private set; } = new Libretro.Wrapper();

        private Renderer _rendererComponent = null;
        private Material _originalMaterial  = null;

        private void OnEnable()
        {
            if (Game != null)
            {
                if (!string.IsNullOrEmpty(Game.Name))
                {
                    if (transform.childCount > 0)
                    {
                        Transform modelTransform = transform.GetChild(0);
                        if (modelTransform != null && modelTransform.childCount > 1)
                        {
                            Transform screenTransform = modelTransform.GetChild(1);
                            if (screenTransform.TryGetComponent(out _rendererComponent))
                            {
                                StartGame();
                            }
                            else
                            {
                                Log.Error($"Screen node '{screenTransform.name}' is missing a 'Renderer' component.", "GameModelSetup.OnEnable");
                            }
                        }
                        else
                        {
                            Log.Error($"Screen node expected on transform '{modelTransform.name}' as second child.", "GameModelSetup.OnEnable");
                        }
                    }
                    else
                    {
                        Log.Error($"Model node expected on transform '{transform.name}' as first child.", "GameModelSetup.OnEnable");
                    }
                }
                else
                {
                    Log.Error("Game name is null or empty.", "GameModelSetup.OnEnable");
                }
            }
            else
            {
                Log.Error("Game is null.", "GameModelSetup.OnEnable");
            }
        }

        private void OnDisable()
        {
            if (_rendererComponent != null && _rendererComponent.material != null && _originalMaterial != null)
            {
                _rendererComponent.material = _originalMaterial;
            }
            StopGame();
        }

        public void ActivateGraphics()
        {
            if (Wrapper.GraphicsProcessor == null)
            {
                Wrapper.GraphicsProcessor = new Libretro.UnityGraphicsProcessor();
            }
        }

        public void DeactivateGraphics()
        {
            Wrapper.GraphicsProcessor = null;
        }

        public void ActivateAudio()
        {
            if (Wrapper.AudioProcessor == null)
            {
                Libretro.UnityAudioProcessorComponent unityAudio = GetComponentInChildren<Libretro.UnityAudioProcessorComponent>();
                if (unityAudio != null)
                {
                    Wrapper.AudioProcessor = unityAudio;
                }
                else
                {
                    Wrapper.AudioProcessor = new Libretro.NAudioAudioProcessor();
                }

                Wrapper.AudioProcessor.Init(Wrapper.Game.SampleRate);
            }
        }

        public void DeactivateAudio()
        {
            Wrapper.AudioProcessor.DeInit();
            Wrapper.AudioProcessor = null;
        }

        public void ActivateInput()
        {
            if (Wrapper.InputProcessor == null)
            {
                Wrapper.InputProcessor = FindObjectOfType<PlayerInputManager>().GetComponent<Libretro.IInputProcessor>();
            }
        }

        public void DeactivateInput()
        {
            Wrapper.InputProcessor = null;
        }

        public void StartGame()
        {
            if (Wrapper != null && Wrapper.Game != null && !Wrapper.Game.Running)
            {
                string corePath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.WrapperDirectory}/cores/{Game.Core}_libretro.dll");
                if (FileSystem.FileExists(corePath))
                {
                    string instancePath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.TempDirectory}/{Path.GetFileName(corePath).Replace("_libretro", $"{Guid.NewGuid()}")}");
                    File.Copy(corePath, instancePath);

                    if (Wrapper.StartCore(instancePath))
                    {
                        if (!string.IsNullOrEmpty(Game.Name))
                        {
                            string gamePath = Wrapper.GetGamePath(Game.Directory, Game.Name);
                            if (gamePath == null)
                            {
                                // Try Zip archive
                                string archivePath = FileSystem.GetAbsolutePath($"{Game.Directory}/{Game.Name}.zip");
                                if (File.Exists(archivePath))
                                {
                                    string extractPath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.TempDirectory}/extracted");
                                    if (Directory.Exists(extractPath))
                                    {
                                        Directory.Delete(extractPath, true);
                                    }
                                    System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
                                    gamePath = Wrapper.GetGamePath(extractPath, Game.Name);
                                }
                            }

                            if (gamePath != null)
                            {
                                if (Wrapper.StartGame(gamePath))
                                {
                                    ActivateGraphics();
                                    ActivateAudio();
                                    ActivateInput();

                                    _originalMaterial = _rendererComponent.sharedMaterial;
                                    _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
                                    _rendererComponent.material.EnableKeyword("_EMISSION");
                                    _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                                    _rendererComponent.material.SetColor("_EmissionColor", Color.white);

                                    InvokeRepeating("LibretroRunLoop", 0f, 1f / Wrapper.Game.Fps);
                                }
                                else
                                {
                                    Log.Error($"Game '{Game.Name}' failed to start on core '{Game.Core}'.", "GameModelSetup.StartGame");
                                }
                            }
                            else
                            {
                                Log.Error($"Game '{Game.Name}' at path '{Game.Directory}' not found.", "GameModelSetup.StartGame");
                            }
                        }
                        else
                        {
                            Log.Warning($"Game not set, running '{Game.Core}' core only.", "GameModelSetup.StartGame");
                        }
                    }
                    else
                    {
                        Log.Warning($"Failed to start core '{Game.Core}'.", "GameModelSetup.StartGame");
                    }
                }
                else
                {
                    Log.Error($"Core '{Game.Core}' at path '{corePath}' not found.", "GameModelSetup.StartGame");
                }
            }
        }

        public void StopGame()
        {
            CancelInvoke();
            Wrapper?.StopGame();
        }

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by InvokeRepeating")]
        private void LibretroRunLoop()
        {
            Wrapper.Update();

            if (Wrapper.GraphicsProcessor != null && Wrapper.GraphicsProcessor is Libretro.UnityGraphicsProcessor unityGraphics)
            {
                if (unityGraphics.Texture != null)
                {
                    _rendererComponent.material.mainTexture = unityGraphics.Texture;
                    _rendererComponent.material.SetTexture("_EmissionMap", unityGraphics.Texture);
                }
            }
        }
    }
}
