using SK.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

namespace SK.Model
{
    [SelectionBase]
    public class GameModelSetup : MonoBehaviour
    {
        public Emulation.Game Game;

        private Renderer _renderer;
        //private Light _light;

        private TextMeshProUGUI _infoText;
        private string _coreInfoText = string.Empty;

        private Emulation.Libretro.Wrapper _wrapper;

        private void Awake()
        {
            _renderer = transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
            //_light   = GetComponent<Light>();

            GameObject uiNode = GameObject.Find("UI");
            if (uiNode != null)
            {
                _infoText = uiNode.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void OnEnable()
        {
            Emulation.Libretro.Wrapper.OnCoreStartedEvent += CoreStartedCallback;
            Emulation.Libretro.Wrapper.OnGameStartedEvent += GameStartedCallback;
            Emulation.Libretro.Wrapper.OnGameStoppedEvent += GameStoppedCallback;
        }

        private void OnDisable()
        {
            Emulation.Libretro.Wrapper.OnCoreStartedEvent -= CoreStartedCallback;
            Emulation.Libretro.Wrapper.OnGameStartedEvent -= GameStartedCallback;
            Emulation.Libretro.Wrapper.OnGameStoppedEvent -= GameStoppedCallback;
        }

        private void OnDestroy()
        {
            StopGame();
        }

        public void SetGame(Emulation.Game game)
        {
            Game = game;
        }

        public void StartGame()
        {
            if (Game == null)
            {
                Log.Error($"Game instance is null.", "GameModelSetup.StartGame");
                return;
            }

            Emulation.Libretro.LibretroAudioSource audioSource = transform.GetComponentInChildren<Emulation.Libretro.LibretroAudioSource>();
            if (audioSource == null)
            {
                Log.Warning($"LibretroAudioSource not found.", "GameModelSetup.StartGame");
            }

            _wrapper = new Emulation.Libretro.Wrapper(audioSource);

            string corePath = FileSystem.GetAlsolutePath($"@Data~/Cores/{Game.Core}_libretro.dll");
            if (FileSystem.FileExists(corePath))
            {
                if (!_wrapper.StartCore(corePath))
                {
                    Log.Error($"Core '{Game.Core}' at path '{corePath}' failed to run.", "GameModelSetup.StartGame");
                }
            }
            else
            {
                Log.Error($"Core '{Game.Core}' at path '{corePath}' not found.", "GameModelSetup.StartGame");
                return;
            }

            if (!string.IsNullOrEmpty(Game.Name))
            {
                string gamePath = _wrapper.GetGamePath(Game.Directory, Game.Name);

                if (gamePath == null)
                {
                    // Zip archive
                    string archivePath = FileSystem.GetAlsolutePath($"{Game.Directory}/{Game.Name}.zip");
                    if (File.Exists(archivePath))
                    {
                        string extractPath = FileSystem.GetAlsolutePath($"{Game.Directory}/temp/");
                        if (Directory.Exists(extractPath))
                        {
                            Directory.Delete(extractPath, true);
                        }
                        System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
                        gamePath = _wrapper.GetGamePath(extractPath, Game.Name);
                    }
                }

                if (gamePath != null)
                {
                    _wrapper.StartGame(gamePath);
                    InvokeRepeating("LibretroRunLoop", 0f, 1f / (float)_wrapper.SystemAVInfo.timing.fps);
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

        public void StopGame()
        {
            if (_wrapper == null)
            {
                return;
            }

            try
            {
                CancelInvoke();
                _wrapper.StopGame();
                _wrapper = null;
            }
            catch (System.Exception e)
            {
                Log.Exception(e.Message);
            }
        }

        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by InvokeRepeating")]
        private void LibretroRunLoop()
        {
            if (_wrapper != null)
            {
                _wrapper.Update();
                if (_wrapper.Texture != null)
                {
                    _renderer.material.mainTexture = _wrapper.Texture;
                    _renderer.material.SetTexture("_EmissionMap", _wrapper.Texture);
                    //_light.color = AverageColorFromTexture(_wrapper.Texture);
                }
            }
        }

        private Material _originalMaterial;

        private void CoreStartedCallback(Emulation.Libretro.Wrapper.CoreStartedInfo info)
        {
            if (_infoText != null)
            {
                StringBuilder sb = new StringBuilder()
                    .AppendLine("<color=yellow>Core Info:</color>")
                    .Append("<size=12>")
                    .AppendLine($"<color=lightblue>Api Version:</color> {info.ApiVersion}")
                    .AppendLine($"<color=lightblue>Name:</color> {info.CoreName}")
                    .AppendLine($"<color=lightblue>Version:</color> {info.CoreVersion}")
                    .AppendLine($"<color=lightblue>ValidExtensions:</color> {info.ValidExtensions}")
                    .AppendLine($"<color=lightblue>RequiresFullPath:</color> {info.RequiresFullPath}")
                    .AppendLine($"<color=lightblue>BlockExtraction:</color> {info.BlockExtraction}")
                    .Append("</size>");
                _coreInfoText = sb.ToString();
                _infoText.SetText(_coreInfoText);
            }
        }

        private void GameStartedCallback(Emulation.Libretro.Wrapper.GameStartedInfo info)
        {
            if (_infoText != null)
            {
                StringBuilder sb = new StringBuilder(_infoText.text)
                    .AppendLine()
                    .AppendLine("<color=yellow>Game Info:</color>")
                    .Append("<size=12>")
                    .AppendLine($"<color=lightblue>BaseWidth:</color> {info.BaseWidth}")
                    .AppendLine($"<color=lightblue>BaseHeight:</color> {info.BaseHeight}")
                    .AppendLine($"<color=lightblue>MaxWidth:</color> {info.MaxWidth}")
                    .AppendLine($"<color=lightblue>MaxHeight:</color> {info.MaxHeight}")
                    .AppendLine($"<color=lightblue>AspectRatio:</color> {info.AspectRatio}")
                    .AppendLine($"<color=lightblue>TargetFps:</color> {info.TargetFps}")
                    .AppendLine($"SampleRate:</color> {info.SampleRate}")
                    .Append("</size>");

                _infoText.SetText(sb.ToString());
            }

            _originalMaterial = _renderer.sharedMaterial;

            _renderer.material.mainTextureScale = new Vector2(1f, -1f);
            _renderer.material.EnableKeyword("_EMISSION");
            _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            _renderer.material.SetColor("_EmissionColor", Color.white);
        }

        private void GameStoppedCallback()
        {
            if (_infoText != null)
            {
                _infoText.SetText(string.Empty);
            }
            _renderer.material = _originalMaterial;
        }

        //private static Color32 AverageColorFromTexture(Texture2D tex)
        //{
        //    Color32[] texColors = tex.GetPixels32();
        //    int total = texColors.Length;
        //    float r = 0;
        //    float g = 0;
        //    float b = 0;
        //    for (int i = 0; i < total; i++)
        //    {
        //        r += texColors[i].r;
        //        g += texColors[i].g;
        //        b += texColors[i].b;
        //    }
        //    return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
        //}
    }
}
