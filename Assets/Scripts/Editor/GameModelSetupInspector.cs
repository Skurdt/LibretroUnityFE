using SK.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SK
{
    [CustomEditor(typeof(Model.GameModelSetup))]
    public class GameModelSetupInspector : Editor
    {
        public Model.GameModelSetup GameModelSetupScript { get; private set; }

        private void OnEnable()
        {
            GameModelSetupScript = target as Model.GameModelSetup;
            LoadConfigFromDisk();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(8f);

            if (GUILayout.Button("Load Existing Configuration", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                LoadConfigFromDisk();
            }

            GUILayout.Space(8f);

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Core", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectCoreWindow();
            }
            GameModelSetupScript.Game.Core = EditorGUILayout.TextField(GameModelSetupScript.Game.Core);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Directory", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectDirectoryDialog();
            }
            GameModelSetupScript.Game.Directory = EditorGUILayout.TextField(GameModelSetupScript.Game.Directory);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Rom", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectRomDialog();
            }
            GameModelSetupScript.Game.Name = EditorGUILayout.TextField(GameModelSetupScript.Game.Name);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8f);

            if (!string.IsNullOrEmpty(GameModelSetupScript.Game.Core))
            {
                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    SaveConfigToDisk();
                }
                if (!EditorApplication.isPlaying)
                {
                    if (GUILayout.Button("Start", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        SaveConfigToDisk();
                        _ = EditorApplication.ExecuteMenuItem("Edit/Play");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No core selected!", MessageType.Error);
            }
        }

        private void SaveConfigToDisk()
        {
            _ = FileSystem.SerializeToJson(new Emulation.GameList
            {
                Games = new Emulation.Game[]
                {
                            new Emulation.Game
                            {
                                Core = GameModelSetupScript.Game.Core,
                                Directory = GameModelSetupScript.Game.Directory,
                                Name = GameModelSetupScript.Game.Name
                            }
                }
            }, "@Data~/Games.json");
        }

        private void LoadConfigFromDisk()
        {
            Emulation.GameList gameList = FileSystem.DeserializeFromJson<Emulation.GameList>("@Data~/Games.json");
            if (gameList != null && gameList.Games.Length > 0)
            {
                GameModelSetupScript.SetGame(gameList.Games[0]);
            }
        }

        private void ShowSelectCoreWindow()
        {
            GameModelSetupSelectCoreWindow window = EditorWindow.GetWindow<GameModelSetupSelectCoreWindow>();
            window.ShowWindow(this);
        }

        private void ShowSelectDirectoryDialog()
        {
            string directory = EditorUtility.OpenFolderPanel("Select rom directory", FileSystem.GetAlsolutePath("@Data~/Roms"), string.Empty);
            if (!string.IsNullOrEmpty(directory))
            {
                GameModelSetupScript.Game.Directory = FileSystem.GetRelativePath(directory);
            }
        }

        private void ShowSelectRomDialog()
        {
            string startingDirectory;
            if (!string.IsNullOrEmpty(GameModelSetupScript.Game.Directory))
            {
                startingDirectory = FileSystem.GetAlsolutePath(GameModelSetupScript.Game.Directory);
            }
            else
            {
                startingDirectory = FileSystem.GetAlsolutePath("@Data~/Roms");
            }

            string filePath = EditorUtility.OpenFilePanel("Select rom", startingDirectory, string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                GameModelSetupScript.Game.Directory = FileSystem.GetRelativePath(Path.GetDirectoryName(filePath));
                GameModelSetupScript.Game.Name = Path.GetFileNameWithoutExtension(filePath);
            }
        }
    }

    public class GameModelSetupSelectCoreWindow : EditorWindow
    {
        private GameModelSetupInspector _gameModelSetupInspector;
        private List<string> _availableCores;
        private GameModelSetupSelectCoreWindow _window;
        private Vector2 _scrollPos;

        public void ShowWindow(GameModelSetupInspector gameModelSetupInspector)
        {
            _gameModelSetupInspector = gameModelSetupInspector;

            _availableCores = new List<string>
            {
                "None"
            };
            string[] corePaths = FileSystem.GetFilesInDirectory("@Data~/Cores", "*.dll");
            foreach (string corePath in corePaths)
            {
                string coreName = Path.GetFileNameWithoutExtension(corePath).Replace("_libretro", string.Empty);
                _availableCores.Add(coreName);
            }

            _window = GetWindow<GameModelSetupSelectCoreWindow>();
            _window.titleContent = new GUIContent
            {
                text = "Select Core"
            };
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
            {
                foreach (string core in _availableCores)
                {
                    if (GUILayout.Button(core))
                    {
                        _gameModelSetupInspector.GameModelSetupScript.Game.Core = core;
                        _window.Close();
                    }
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
