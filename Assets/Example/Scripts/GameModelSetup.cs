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

// using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Common
{
    [System.Serializable]
    public class Game
    {
        public string Core      = "mame2003_plus";
        public string Directory = "D:/mame2003-plus/roms";
        public string Name      = "pacman";
    }

    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public Libretro.Wrapper Wrapper { get; private set; }
        public bool InputEnabled { get; private set; }

        [HideInInspector] public Game Game;

        [SerializeField] private PlayerControls _player;
        [SerializeField] [Range(0.5f, 2f)] private float _timeScale = 1.0f;
        [SerializeField] private bool _useAudioRateForSync          = false;  // Very dirty workaround... FIX ME!
        [SerializeField] private float _audioMinDistance            = 2f;
        [SerializeField] private float _audioMaxDistance            = 10f;
        [SerializeField] private FilterMode _videoFilterMode        = FilterMode.Point;
        [SerializeField] private bool _cropOverscan                 = true;

        private Transform _screenTransform   = null;
        private Renderer _rendererComponent = null;
        private Material _originalMaterial  = null;

        private float _gameFps         = 0f;
        private float _gameSampleRate  = 0f;

        private float _frameTimer      = 0f;
        private float _audioVolume     = 1f;

        private void OnEnable()
        {
            StartGame();
        }

        private void OnDisable()
        {
            StopGame();
        }

        private void Update()
        {
            if (Wrapper != null && _gameFps > 0f && _gameSampleRate > 0f)
            {
                _frameTimer += Time.deltaTime;
                float targetFrameTime = 1f / (_useAudioRateForSync ?_gameSampleRate / 1000f : _gameFps) / _timeScale;
                while (_frameTimer >= targetFrameTime)
                {
                    Wrapper.Update();
                    _frameTimer -= targetFrameTime;
                }

                VideoSetFilterMode(_videoFilterMode);

                if (Wrapper.AudioProcessor != null && Wrapper.AudioProcessor is Libretro.NAudioAudioProcessor NAudioAudio)
                {
                    float distance = Vector3.Distance(transform.position, _player.transform.position);
                    if (distance > 0f)
                    {
                        _audioVolume = math.clamp(math.pow((distance - _audioMaxDistance) / (_audioMinDistance - _audioMaxDistance), 2f), 0f, 1f);
                        NAudioAudio.SetVolume(_audioVolume);
                    }
                    else
                    {
                        NAudioAudio.SetVolume(1f);
                    }
                }
            }
        }

        public void StartGame()
        {
            if (Game != null && !string.IsNullOrEmpty(Game.Core))
            {
                if (transform.childCount > 0)
                {
                    Transform modelTransform = transform.GetChild(0);
                    if (modelTransform != null && modelTransform.childCount > 1)
                    {
                        _screenTransform = modelTransform.GetChild(1);
                        if (_screenTransform.TryGetComponent(out _rendererComponent))
                        {
                            Wrapper = new Libretro.Wrapper((Libretro.TargetPlatform)Application.platform)
                            {
                                OptionCropOverscan = _cropOverscan
                            };

                            if (Wrapper.StartGame(Game.Core, Game.Directory, Game.Name))
                            {
                                _gameFps = (float)Wrapper.Game.SystemAVInfo.timing.fps;
                                _gameSampleRate = (float)Wrapper.Game.SystemAVInfo.timing.sample_rate;

                                ActivateGraphics();
                                ActivateAudio();

                                _originalMaterial = _rendererComponent.sharedMaterial;
                                _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
                                _rendererComponent.material.color = Color.black;
                                _rendererComponent.material.EnableKeyword("_EMISSION");
                                _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                                _rendererComponent.material.SetColor("_EmissionColor", Color.white);
                            }
                            else
                            {
                                Wrapper.StopGame();
                                Wrapper = null;
                            }
                        }
                    }
                }
            }
        }

        public void StopGame()
        {
            if (_rendererComponent != null && _rendererComponent.material != null && _originalMaterial != null)
            {
                _rendererComponent.material = _originalMaterial;
            }

            Wrapper?.StopGame();
            Wrapper = null;
        }

        public void ActivateGraphics()
        {
            Libretro.UnityGraphicsProcessor unityGraphics = new Libretro.UnityGraphicsProcessor
            {
                OnTextureRecreated = VideoSetTextureCallback,
                VideoFilterMode    = _videoFilterMode
            };
            Wrapper?.ActivateGraphics(unityGraphics);
        }

        public void DeactivateGraphics()
        {
            Wrapper?.DeactivateGraphics();
        }

        public void ActivateAudio()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                {
                    Libretro.UnityAudioProcessorComponent unityAudio = GetComponentInChildren<Libretro.UnityAudioProcessorComponent>();
                    if (unityAudio != null)
                    {
                        Wrapper?.ActivateAudio(unityAudio);
                    }
                    else
                    {
                        Wrapper?.ActivateAudio(new Libretro.NAudioAudioProcessor());
                    }
                }
                break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                {
                    Libretro.UnityAudioProcessorComponent unityAudio = GetComponentInChildren<Libretro.UnityAudioProcessorComponent>(true);
                    if (unityAudio != null)
                    {
                        unityAudio.gameObject.SetActive(true);
                        Wrapper?.ActivateAudio(unityAudio);
                    }
                    else
                    {
                        GameObject audioProcessorGameObject = new GameObject("AudioProcessor", typeof(Libretro.UnityAudioProcessorComponent));
                        audioProcessorGameObject.transform.SetParent(_screenTransform);
                        Wrapper?.ActivateAudio(audioProcessorGameObject.GetComponent<Libretro.UnityAudioProcessorComponent>());
                    }
                }
                break;
            }
        }

        public void DeactivateAudio()
        {
            Wrapper?.DeactivateAudio();
        }

        public void ActivateInput()
        {
            InputEnabled = true;
            Wrapper?.ActivateInput(FindObjectOfType<PlayerInputManager>().GetComponent<Libretro.IInputProcessor>());
        }

        public void DeactivateInput()
        {
            InputEnabled = false;
            Wrapper?.DeactivateInput();
        }

        public void VideoSetFilterMode(FilterMode filterMode)
        {
            if (Wrapper != null && Wrapper.GraphicsProcessor != null && Wrapper.GraphicsProcessor is Libretro.UnityGraphicsProcessor unityGraphics)
            {
                unityGraphics.VideoFilterMode = filterMode;
            }
        }

        private void VideoSetTextureCallback(Texture2D texture)
        {
            if (_rendererComponent != null)
            {
                _rendererComponent.material.SetTexture("_EmissionMap", texture);
            }
        }
    }
}
