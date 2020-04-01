using SK.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    [System.Serializable]
    public class Game
    {
        public string Core;
        public string Directory;
        public string Name;
    }

    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public Libretro.Wrapper Wrapper { get; private set; }

        public Game Game;

        private Renderer _rendererComponent = null;
        private Material _originalMaterial = null;

        private TextMeshProUGUI _infoTextComponent = null;
        private string _coreInfoText = string.Empty;

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Constructor subscribes to events, no methods are called directly on this...")]
        private AudioDeviceExternal _externalAudio = null;

        private double _updateLoopTimeMs;

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

                        Game = FileSystem.DeserializeFromJson<Game>($"{Application.streamingAssetsPath}/Game.json");

                        AudioSource foundUnityAudioSource = GetComponentInChildren<AudioSource>();
                        if (foundUnityAudioSource == null)
                        {
                            _externalAudio = new AudioDeviceExternal();
                        }
                    }
                }
            }

            GameObject uiNode = GameObject.Find("UI");
            if (uiNode != null)
            {
                _infoTextComponent = uiNode.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void Start()
        {
            StartGame();
        }

        private void OnEnable()
        {
            Libretro.Wrapper.OnCoreStartedEvent += CoreStartedCallback;
            Libretro.Wrapper.OnCoreStoppedEvent += CoreStoppedCallback;
            Libretro.Wrapper.OnGameStartedEvent += GameStartedCallback;
            Libretro.Wrapper.OnGameStoppedEvent += GameStoppedCallback;
        }

        private void OnDisable()
        {
            Libretro.Wrapper.OnCoreStartedEvent -= CoreStartedCallback;
            Libretro.Wrapper.OnCoreStoppedEvent -= CoreStoppedCallback;
            Libretro.Wrapper.OnGameStartedEvent -= GameStartedCallback;
            Libretro.Wrapper.OnGameStoppedEvent -= GameStoppedCallback;
        }

        private void OnDestroy()
        {
            StopGame();
        }

        private void Update()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                Cursor.visible = !Cursor.visible;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopGame();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(0);
#endif
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(4, 2, 200, 100), $"Update: {_updateLoopTimeMs}ms");
        }

        private void StartGame()
        {
            if (Game != null)
            {
                Wrapper = new Libretro.Wrapper();

                string corePath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.WrapperDirectory}/cores/{Game.Core}_libretro.dll");
                if (FileSystem.FileExists(corePath))
                {
                    Wrapper.StartCore(corePath);

                    if (!string.IsNullOrEmpty(Game.Name))
                    {
                        string gamePath = Wrapper.GetGamePath(Game.Directory, Game.Name);
                        if (gamePath == null)
                        {
                            // Try Zip archive
                            string archivePath = FileSystem.GetAbsolutePath($"{Game.Directory}/{Game.Name}.zip");
                            if (File.Exists(archivePath))
                            {
                                string extractPath = FileSystem.GetAbsolutePath($"{Libretro.Wrapper.WrapperDirectory}/temp");
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
                            Wrapper.StartGame(gamePath);
                            InvokeRepeating("LibretroRunLoop", 0f, 1f / Wrapper.Game.Fps);
                        }
                        else
                        {
                            Log.Error($"Game '{Game.Name}' at path '{Game.Directory}' not found.", "GameModelSetup.StartGame");
                        }
                    }
                    else
                    {
                        Log.Warning($"Game not set, running core only.", "GameModelSetup.StartGame");
                    }
                }
                else
                {
                    Log.Error($"Core '{Game.Core}' at path '{corePath}' not found.", "GameModelSetup.StartGame");
                }
            }
            else
            {
                Log.Error($"Game instance is null.", "GameModelSetup.StartGame");
            }
        }

        public void StopGame()
        {
            if (Wrapper != null)
            {
                CancelInvoke();
                Wrapper.StopGame();
                Wrapper = null;
            }
        }

        private int numFrames;

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by InvokeRepeating")]
        private void LibretroRunLoop()
        {
            if (Wrapper != null)
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                Wrapper.Update();

                sw.Stop();
                if (numFrames++ >= 10)
                {
                    _updateLoopTimeMs = sw.Elapsed.TotalMilliseconds;
                    numFrames = 0;
                }

                if (Wrapper.Texture != null)
                {
                    _rendererComponent.material.mainTexture = Wrapper.Texture;
                    _rendererComponent.material.SetTexture("_EmissionMap", Wrapper.Texture);
                }
            }
        }

        private void CoreStartedCallback(Libretro.Wrapper.LibretroCore core)
        {
            if (core != null)
            {
                if (_infoTextComponent != null)
                {
                    StringBuilder sb = new StringBuilder()
                        .AppendLine("<color=yellow>Core Info:</color>")
                        .Append("<size=12>")
                        .AppendLine($"<color=lightblue>Api Version:</color> {core.ApiVersion}")
                        .AppendLine($"<color=lightblue>Name:</color> {core.CoreName}")
                        .AppendLine($"<color=lightblue>Version:</color> {core.CoreVersion}")
                        .AppendLine($"<color=lightblue>ValidExtensions:</color> {string.Join("|", core.ValidExtensions)}")
                        .AppendLine($"<color=lightblue>RequiresFullPath:</color> {core.RequiresFullPath}")
                        .AppendLine($"<color=lightblue>BlockExtraction:</color> {core.BlockExtraction}")
                        .Append("</size>");
                    _coreInfoText = sb.ToString();
                    _infoTextComponent.SetText(_coreInfoText);
                }
            }
        }

        private void CoreStoppedCallback(Libretro.Wrapper.LibretroCore core)
        {
            if (_infoTextComponent != null)
            {
                _infoTextComponent.SetText(string.Empty);
            }
        }

        private void GameStartedCallback(Libretro.Wrapper.LibretroGame game)
        {
            if (game != null)
            {
                if (_infoTextComponent != null)
                {
                    StringBuilder sb = new StringBuilder(_infoTextComponent.text)
                        .AppendLine()
                        .AppendLine("<color=yellow>Game Info:</color>")
                        .Append("<size=12>")
                        .AppendLine($"<color=lightblue>BaseWidth:</color> {game.BaseWidth}")
                        .AppendLine($"<color=lightblue>BaseHeight:</color> {game.BaseHeight}")
                        .AppendLine($"<color=lightblue>MaxWidth:</color> {game.MaxWidth}")
                        .AppendLine($"<color=lightblue>MaxHeight:</color> {game.MaxHeight}")
                        .AppendLine($"<color=lightblue>AspectRatio:</color> {game.AspectRatio}")
                        .AppendLine($"<color=lightblue>TargetFps:</color> {game.Fps}")
                        .AppendLine($"<color=lightblue>SampleRate:</color> {game.SampleRate}")
                        .Append("</size>");

                    _infoTextComponent.SetText(sb.ToString());
                }
            }

            _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
            _rendererComponent.material.EnableKeyword("_EMISSION");
            _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            _rendererComponent.material.SetColor("_EmissionColor", Color.white);
        }

        private void GameStoppedCallback(Libretro.Wrapper.LibretroGame game)
        {
            _rendererComponent.material = _originalMaterial;

            if (_infoTextComponent != null)
            {
                _infoTextComponent.SetText(string.Empty);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
