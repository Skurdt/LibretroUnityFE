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

using SK.Libretro;
using System.Collections;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    [System.Serializable]
    public class Game
    {
        public string Core      = "mame2003_plus";
        public string Directory = "D:/mame2003-plus/roms";
        public string Name      = "pacman";
    }

    [System.Serializable]
    public class GameConfigFile
    {
        public bool UseConfig;
        public Game Game;
    }

    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public LibretroWrapper Wrapper { get; private set; }

        public bool VideoEnabled
        {
            get => _graphicsEnabled;
            set
            {
                if (value)
                {
                    ActivateGraphics();
                }
                else
                {
                    DeactivateGraphics();
                }
            }
        }

        public bool AudioEnabled
        {
            get => _audioEnabled;
            set
            {
                if (value)
                {
                    ActivateAudio();
                }
                else
                {
                    DeactivateAudio();
                }
            }
        }

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                if (value)
                {
                    ActivateInput();
                }
                else
                {
                    DeactivateInput();
                }
            }
        }

        public bool AudioVolumeControlledByDistance { get; set; } = true;

        public float AudioMaxVolume
        {
            get => _audioMaxVolume;
            set
            {
                _audioMaxVolume = value;
                AudioSetVolume(value);
            }
        }

        [HideInInspector] public Game Game;

        [SerializeField, Range(0.5f, 2f)] private float _timeScale    = 1.0f;
        [SerializeField, Range(0f, 1f)] private float _audioMaxVolume = 1f;
        [SerializeField] private float _audioMinDistance              = 2f;
        [SerializeField] private float _audioMaxDistance              = 10f;
        [SerializeField] private FilterMode _videoFilterMode          = FilterMode.Point;

        private Player.Controls _player;

        private Transform _screenTransform   = null;
        private Renderer _rendererComponent  = null;
        private MaterialPropertyBlock _block = null;

        private bool _graphicsEnabled;
        private bool _audioEnabled;
        private bool _inputEnabled;

        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private readonly int _maxSkipFrames = 10;
        private double _targetFrameTime     = 0.0;
        private double _accumulatedTime     = 0.0;
        private int _nLoops                 = 0;

        [ContextMenu("Load configuration")]
        public void EditorLoadConfig()
        {
            string text        = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "game.json")));
            GameConfigFile cfg = JsonUtility.FromJson<GameConfigFile>(text);
            if (cfg != null)
            {
                Game = cfg.Game;
            }
        }

        [ContextMenu("Save configuration")]
        public void EditorSaveConfig()
        {
            string json = JsonUtility.ToJson(new GameConfigFile { UseConfig = true, Game = Game }, true);
            File.WriteAllText(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "game.json")), json);
        }

        private void Awake() => _player = FindObjectOfType<Player.Controls>();

        private IEnumerator Start()
        {
            string text        = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "game.json")));
            GameConfigFile cfg = JsonUtility.FromJson<GameConfigFile>(text);
            if (cfg != null && cfg.UseConfig)
            {
                Game = cfg.Game;
            }

            if (!StartGame())
            {
                yield break;
            }

            yield return CoUpdateGame();
        }

        private void OnDisable() => StopGame();

        private bool StartGame()
        {
            if (Game == null || string.IsNullOrEmpty(Game.Core))
            {
                return false;
            }

            if (transform.childCount == 0)
            {
                return false;
            }

            Transform modelTransform = transform.GetChild(0);
            if (modelTransform == null || modelTransform.childCount == 1)
            {
                return false;
            }

            _screenTransform = modelTransform.GetChild(1);
            if (!_screenTransform.TryGetComponent(out _rendererComponent))
            {
                return false;
            }

            Wrapper = new LibretroWrapper((LibretroTargetPlatform)Application.platform, $"{Application.streamingAssetsPath}/libretro~");

            if (!Wrapper.StartGame(Game.Core, Game.Directory, Game.Name))
            {
                StopGame();
                return false;
            }

            ActivateGraphics();
            ActivateAudio();

            _block = new MaterialPropertyBlock();
            _rendererComponent.GetPropertyBlock(_block);
            _block.SetFloat("_Intensity", 1.1f);
            _block.SetInt("_Rotation", Wrapper.Core.Rotation);
            _rendererComponent.SetPropertyBlock(_block);

            return true;
        }

        private void StopGame()
        {
            Wrapper?.StopGame();

            Wrapper = null;
            _block  = null;
        }

        private IEnumerator CoUpdateGame()
        {
            while (Wrapper != null)
            {
                yield return null;

                _targetFrameTime  = 1.0 / Wrapper.Game.VideoFps / _timeScale;
                _accumulatedTime += _stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();
                _nLoops = 0;

                while (_accumulatedTime >= _targetFrameTime && _nLoops < _maxSkipFrames)
                {
                    Wrapper.Update();
                    _accumulatedTime -= _targetFrameTime;
                    ++_nLoops;
                    yield return null;
                }

                GraphicsSetFilterMode(_videoFilterMode);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (AudioVolumeControlledByDistance && Wrapper.Audio.Processor is Libretro.NAudio.AudioProcessor NAudioAudio)
                {
                    float distance = Vector3.Distance(transform.position, _player.transform.position);
                    if (distance > 0f)
                    {
                        float volume = math.clamp(math.pow((distance - _audioMaxDistance) / (_audioMinDistance - _audioMaxDistance), 2f), 0f, _audioMaxVolume);
                        NAudioAudio.SetVolume(volume);
                    }
                }
#endif
            }
        }

        private void GraphicsSetFilterMode(FilterMode filterMode)
        {
            if (Wrapper?.Video.Processor is Libretro.Unity.GraphicsProcessor unityGraphics && unityGraphics.Texture != null)
            {
                unityGraphics.VideoFilterMode = filterMode;
            }
        }

        private void ActivateGraphics()
        {
            Libretro.Unity.GraphicsProcessor unityGraphics = new Libretro.Unity.GraphicsProcessor
            {
                OnTextureRecreated = GraphicsSetTextureCallback,
                VideoFilterMode    = _videoFilterMode
            };
            Wrapper?.ActivateGraphics(unityGraphics);
            _graphicsEnabled = true;
        }

        private void DeactivateGraphics()
        {
            Wrapper?.DeactivateGraphics();
            _graphicsEnabled = false;
        }

        private void ActivateAudio()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            Libretro.Unity.AudioProcessor unityAudio = GetComponentInChildren<Libretro.Unity.AudioProcessor>();
            if (unityAudio != null)
            {
                Wrapper?.ActivateAudio(unityAudio);
            }
            else
            {
                Wrapper?.ActivateAudio(new Libretro.NAudio.AudioProcessor());
            }
#else
            Libretro.Unity.AudioProcessor unityAudio = GetComponentInChildren<Libretro.Unity.AudioProcessor>(true);
            if (unityAudio != null)
            {
                unityAudio.gameObject.SetActive(true);
            }
            else
            {
                GameObject audioProcessorGameObject = new GameObject("AudioProcessor");
                audioProcessorGameObject.transform.SetParent(_screenTransform);
                unityAudio = audioProcessorGameObject.AddComponent<Libretro.Unity.AudioProcessor>();
            }

            Wrapper?.ActivateAudio(unityAudio);
#endif
            _audioEnabled = true;
        }

        private void DeactivateAudio()
        {
            Wrapper?.DeactivateAudio();
            _audioEnabled = false;
        }

        private void ActivateInput()
        {
            Wrapper?.ActivateInput(FindObjectOfType<PlayerInputManager>().GetComponent<IInputProcessor>());
            _inputEnabled = true;
        }

        private void DeactivateInput()
        {
            Wrapper?.DeactivateInput();
            _inputEnabled = false;
        }

        private void GraphicsSetTextureCallback(Texture2D texture)
        {
            if (_block != null && _rendererComponent != null && texture != null)
            {
                _block.SetTexture("_Texture", texture);
                _rendererComponent.SetPropertyBlock(_block);
            }
        }

        private void AudioSetVolume(float volume)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (Wrapper.Audio.Processor != null && Wrapper.Audio.Processor is Libretro.NAudio.AudioProcessor NAudioAudio)
            {
                NAudioAudio.SetVolume(math.clamp(volume, 0f, _audioMaxVolume));
            }
#endif
        }
    }
}
