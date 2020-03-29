using SK.Utilities;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SK
{
    [CustomEditor(typeof(GameModelSetup))]
    public class GameModelSetupInspector : Editor
    {
        public GameModelSetup GameModelSetupScript { get; private set; }

        private static readonly string _gameConfigFile = $"{Application.streamingAssetsPath}/Game.json";

        private void OnEnable()
        {
            GameModelSetupScript = target as GameModelSetup;
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
                ShowSelectRomDirectoryDialog();
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
            _ = FileSystem.SerializeToJson(GameModelSetupScript.Game, _gameConfigFile);
        }

        private void LoadConfigFromDisk()
        {
            GameModelSetupScript.Game = FileSystem.DeserializeFromJson<Game>(_gameConfigFile);
        }

        private void ShowSelectCoreWindow()
        {
            string startingDirectory = FileSystem.GetAbsolutePath(Libretro.Wrapper.CoresDirectory);
            string filePath = EditorUtility.OpenFilePanel("Select core", startingDirectory, "dll");
            if (!string.IsNullOrEmpty(filePath))
            {
                GameModelSetupScript.Game.Core = Path.GetFileNameWithoutExtension(filePath).Replace("_libretro", string.Empty);
            }
        }

        private void ShowSelectRomDirectoryDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(GameModelSetupScript.Game.Directory))
            {
                startingDirectory = FileSystem.GetAbsolutePath(GameModelSetupScript.Game.Directory);
            }

            string directory = EditorUtility.OpenFolderPanel("Select rom directory", startingDirectory, startingDirectory);
            if (!string.IsNullOrEmpty(directory))
            {
                GameModelSetupScript.Game.Directory = FileSystem.GetRelativePath(directory);
            }
        }

        private void ShowSelectRomDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(GameModelSetupScript.Game.Directory))
            {
                startingDirectory = FileSystem.GetAbsolutePath(GameModelSetupScript.Game.Directory);
            }

            string filePath = EditorUtility.OpenFilePanel("Select rom", startingDirectory, string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                GameModelSetupScript.Game.Directory = FileSystem.GetRelativePath(Path.GetDirectoryName(filePath));
                GameModelSetupScript.Game.Name = Path.GetFileNameWithoutExtension(filePath);
            }
        }
    }
}
