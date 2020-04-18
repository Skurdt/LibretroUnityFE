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
        [HideInInspector] public Game Game;

        [SerializeField] [Range(0.5f, 2f)] private float _timeScale = 1.0f;
        [SerializeField] private bool _cropOverscan = true;

        public Libretro.Wrapper Wrapper { get; private set; }

        private Renderer _rendererComponent = null;
        private Material _originalMaterial = null;

        private float _frameTimer;

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
            if (Wrapper != null && Wrapper.Game.SystemAVInfo.timing.fps > 0.0)
            {
                _frameTimer += Time.deltaTime;
                float timePerFrame = 1f / (float)Wrapper.Game.SystemAVInfo.timing.fps / _timeScale;

                while (_frameTimer >= timePerFrame)
                {
                    Wrapper.Update();
                    _frameTimer -= timePerFrame;
                }

                if (Wrapper.GraphicsProcessor != null && Wrapper.GraphicsProcessor is Libretro.UnityGraphicsProcessor unityGraphics)
                {
                    if (unityGraphics.TextureUpdated)
                    {
                        _rendererComponent.material.SetTexture("_EmissionMap", unityGraphics.Texture);
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
                        Transform screenTransform = modelTransform.GetChild(1);
                        if (screenTransform.TryGetComponent(out _rendererComponent))
                        {
                            Wrapper = new Libretro.Wrapper
                            {
                                OptionCropOverscan = _cropOverscan
                            };

                            if (Wrapper.StartGame(Game.Core, Game.Directory, Game.Name))
                            {
                                ActivateGraphics();
                                ActivateAudio();
                                ActivateInput();

                                _originalMaterial = _rendererComponent.sharedMaterial;
                                _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
                                _rendererComponent.material.color = Color.black;
                                _rendererComponent.material.EnableKeyword("_EMISSION");
                                _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                                _rendererComponent.material.SetColor("_EmissionColor", Color.white);
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
            Wrapper?.ActivateGraphics(new Libretro.UnityGraphicsProcessor());
        }

        public void DeactivateGraphics()
        {
            Wrapper?.DeactivateGraphics();
        }

        public void ActivateAudio()
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

        public void DeactivateAudio()
        {
            Wrapper?.DeactivateAudio();
        }

        public void ActivateInput()
        {
            Wrapper?.ActivateInput(FindObjectOfType<PlayerInputManager>().GetComponent<Libretro.IInputProcessor>());
        }

        public void DeactivateInput()
        {
            Wrapper?.DeactivateInput();
        }

        public void SetVideoUseFiltering(bool filtering)
        {
            Libretro.UnityGraphicsProcessor.VideoFilterMode = filtering ? FilterMode.Trilinear : FilterMode.Point;
            GameObject.Find("VideoFilteringValue").GetComponent<UnityEngine.UI.Text>().text = filtering.ToString();
        }
    }
}
