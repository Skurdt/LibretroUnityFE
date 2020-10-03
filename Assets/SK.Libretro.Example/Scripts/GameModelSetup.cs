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

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static SK.Libretro.LibretroStructs;

namespace SK.Examples
{
    [Serializable]
    public struct Game
    {
        public string Core;
        public string Directory;
        public string Name;
    }

    [Serializable]
    public struct ConfigFile
    {
        public bool DigitalInputAsAnalog;
        public Game Game;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InitContextData
    {
        public IntPtr Handle;
        public int Width;
        public int Height;
        [MarshalAs(UnmanagedType.U1)] public bool Depth;
        [MarshalAs(UnmanagedType.U1)] public bool Stencil;

        public InitContextData(Texture texture, ref retro_hw_render_callback cb)
        {
            Handle  = texture.GetNativeTexturePtr();
            Width   = texture.width;
            Height  = texture.height;
            Depth   = cb.depth;
            Stencil = cb.stencil;
        }
    }

    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public Libretro.LibretroWrapper Wrapper { get; private set; }

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
        [SerializeField] private bool _digitalDirectionsAsAnalog           = false;

        private Player.Controls _player;

        private Transform _screenTransform  = null;
        private Renderer _rendererComponent = null;

        private bool _graphicsEnabled;
        private bool _audioEnabled;
        private bool _inputEnabled;

        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private readonly int _maxSkipFrames                      = 10;
        private double _targetFrameTime                          = 0.0;
        private double _accumulatedTime                          = 0.0;
        private int _nLoops                                      = 0;

        private Coroutine _co;
        private bool _gameRunning = false;

        private bool _contextInitialized             = false;
        private CommandBuffer _retroRunCommandBuffer = null;

        private void Awake() => _player = FindObjectOfType<Player.Controls>();

        private void Start()
        {
            LoadConfig();
            _gameRunning = StartGame();
        }

        private void OnDisable() => StopGame();

        private void Update()
        {
            bool haveKeyboard = Keyboard.current != null;

            if (haveKeyboard && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (_co != null)
                {
                    StopCoroutine(_co);
                }

                StopGame();

                Utils.ExitApp();
            }

            if (_co == null && Wrapper != null && _gameRunning)
            {
                _co = StartCoroutine(CoUpdate());
            }

            GraphicsSetFilterMode(_videoFilterMode);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (!AudioVolumeControlledByDistance || Wrapper == null || Wrapper.Audio == null || !(Wrapper.Audio.Processor is Libretro.NAudio.AudioProcessor audioProcessor))
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance > 0f)
            {
                float volume = math.clamp(math.pow((distance - _audioMaxDistance) / (_audioMinDistance - _audioMaxDistance), 2f), 0f, _audioMaxVolume);
                audioProcessor.SetVolume(volume);
            }
#endif
        }

        private bool StartGame()
        {
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

            Wrapper = new Libretro.LibretroWrapper((Libretro.LibretroTargetPlatform)Application.platform, $"{Application.streamingAssetsPath}/libretro~");
            if (!Wrapper.StartGame(Game.Core, Game.Directory, Game.Name))
            {
                StopGame();
                return false;
            }

            ActivateGraphics();
            ActivateAudio();

            _accumulatedTime = 0;
            _stopwatch.Restart();

            return true;
        }

        private void StopGame()
        {
            DeactivateGraphics();

            Wrapper?.StopGame();
            Wrapper = null;
            _gameRunning = false;
        }

        private IEnumerator CoUpdate()
        {
            if (Wrapper.Core.HwAccelerated)
            {
                if (!_contextInitialized)
                {
                    yield break;
                }

                _retroRunCommandBuffer.Clear();
            }

            Wrapper.FrameTimeUpdate();

            _targetFrameTime  = 1.0 / Wrapper.Game.VideoFps / _timeScale;
            _accumulatedTime += _stopwatch.Elapsed.TotalSeconds;
            _nLoops           = 0;
            _stopwatch.Restart();

            while (_accumulatedTime >= _targetFrameTime && _nLoops < _maxSkipFrames)
            {
                if (Wrapper.Core.HwAccelerated)
                {
                    _retroRunCommandBuffer.IssuePluginEvent(Libretro.LibretroPlugin.GetRenderEventFunc(), 2);
                }
                else
                {
                    Wrapper.Update();
                }
                _accumulatedTime -= _targetFrameTime;
                ++_nLoops;
                yield return null;
            }

            if (Wrapper.Core.HwAccelerated)
            {
                yield return new WaitForEndOfFrame();
                Graphics.ExecuteCommandBuffer(_retroRunCommandBuffer);
            }

            _co = null;
        }

        private void ActivateGraphics()
        {
            if (Wrapper == null)
            {
                return;
            }

            TextureFormat textureFormat = Wrapper.Core.HwAccelerated ? TextureFormat.RGB24 : TextureFormat.BGRA32;
            Libretro.Unity.GraphicsProcessor unityGraphics = new Libretro.Unity.GraphicsProcessor(Wrapper.Game.VideoWidth, Wrapper.Game.VideoHeight, textureFormat)
            {
                OnTextureRecreated = GraphicsSetTextureCallback
            };

            Wrapper.ActivateGraphics(unityGraphics);

            if (Wrapper.Core.HwAccelerated && SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
            {
                InitContextData initContextData = new InitContextData(unityGraphics.Texture, ref Wrapper.HwRenderInterface);
                CommandBuffer cb = new CommandBuffer();
                unsafe
                {
                    cb.IssuePluginEventAndData(Libretro.LibretroPlugin.GetRenderEventFunc(), 0, (IntPtr)(&initContextData));
                }
                Graphics.ExecuteCommandBuffer(cb);

                GraphicsSetTextureCallback(unityGraphics.Texture);

                _retroRunCommandBuffer = new CommandBuffer();
                _contextInitialized = true;
            }

            _graphicsEnabled = true;
        }

        private void DeactivateGraphics()
        {
            _graphicsEnabled = false;

            if (Wrapper == null)
            {
                return;
            }

            if (Wrapper.Core.HwAccelerated && _contextInitialized)
            {
                CommandBuffer cb = new CommandBuffer();
                cb.IssuePluginEvent(Libretro.LibretroPlugin.GetRenderEventFunc(), 1);
                Graphics.ExecuteCommandBuffer(cb);
                _contextInitialized = false;
            }

            Wrapper.DeactivateGraphics();
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
            Libretro.IInputProcessor processor = FindObjectOfType<PlayerInputManager>().GetComponent<Libretro.IInputProcessor>();
            if (processor != null)
            {
                processor.DigitalDirectionsAsAnalog = _digitalDirectionsAsAnalog;
                Wrapper?.ActivateInput(processor);
            }
            _inputEnabled = true;
        }

        private void DeactivateInput()
        {
            Wrapper?.DeactivateInput();
            _inputEnabled = false;
        }

        private void GraphicsSetFilterMode(FilterMode filterMode)
        {
            if (Wrapper?.Video.Processor is Libretro.Unity.GraphicsProcessor unityGraphics && unityGraphics.Texture != null)
            {
                unityGraphics.VideoFilterMode = filterMode;
            }
        }

        private void GraphicsSetTextureCallback(Texture2D texture)
        {
            if (_rendererComponent == null || texture == null)
            {
                return;
            }

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _rendererComponent.GetPropertyBlock(block);
            block.SetTexture("_MainTex", texture);
            block.SetColor("_Color", Color.white);
            _rendererComponent.SetPropertyBlock(block);
        }

        private void AudioSetVolume(float volume)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (Wrapper != null && Wrapper.Audio != null && Wrapper.Audio.Processor is Libretro.NAudio.AudioProcessor audioProcessor)
            {
                audioProcessor.SetVolume(math.clamp(volume, 0f, _audioMaxVolume));
            }
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Load configuration")]
#endif
        private void LoadConfig()
        {
            string text = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "config.json")));
            ConfigFile cfg = JsonUtility.FromJson<ConfigFile>(text);

            Game = cfg.Game;
            _digitalDirectionsAsAnalog = cfg.DigitalInputAsAnalog;
        }

#if UNITY_EDITOR
        [ContextMenu("Save configuration")]
        private void EditorSaveConfig()
        {
            string json = JsonUtility.ToJson(new ConfigFile { DigitalInputAsAnalog = _digitalDirectionsAsAnalog, Game = Game }, true);
            File.WriteAllText(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "config.json")), json);
        }
#endif
    }
}
