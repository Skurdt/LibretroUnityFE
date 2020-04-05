using System.IO;
using UnityEditor;
using UnityEngine;
using static SK.Libretro.Utilities.FileSystem;

namespace SK
{
    [CustomEditor(typeof(Game))]
    public class GameInspector : Editor
    {
        public Game GameScriptableObject { get; private set; }

        private void OnEnable()
        {
            GameScriptableObject = target as Game;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(8f);

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Core", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectCoreWindow();
            }
            GameScriptableObject.Core = EditorGUILayout.TextField(GameScriptableObject.Core);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Directory", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectRomDirectoryDialog();
            }
            GameScriptableObject.Directory = EditorGUILayout.TextField(GameScriptableObject.Directory);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Rom", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                ShowSelectRomDialog();
            }
            GameScriptableObject.Name = EditorGUILayout.TextField(GameScriptableObject.Name);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8f);

            if (string.IsNullOrEmpty(GameScriptableObject.Core))
            {
                EditorGUILayout.HelpBox("No core selected!", MessageType.Error);
            }
        }

        private void ShowSelectCoreWindow()
        {
            string startingDirectory = GetAbsolutePath(Libretro.Wrapper.CoresDirectory);
            string filePath = EditorUtility.OpenFilePanel("Select core", startingDirectory, "dll");
            if (!string.IsNullOrEmpty(filePath))
            {
                GameScriptableObject.Core = Path.GetFileNameWithoutExtension(filePath).Replace("_libretro", string.Empty);
            }
        }

        private void ShowSelectRomDirectoryDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(GameScriptableObject.Directory))
            {
                startingDirectory = GetAbsolutePath(GameScriptableObject.Directory);
            }

            string directory = EditorUtility.OpenFolderPanel("Select rom directory", startingDirectory, startingDirectory);
            if (!string.IsNullOrEmpty(directory))
            {
                GameScriptableObject.Directory = GetRelativePath(directory);
            }
        }

        private void ShowSelectRomDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(GameScriptableObject.Directory))
            {
                startingDirectory = GetAbsolutePath(GameScriptableObject.Directory);
            }

            string filePath = EditorUtility.OpenFilePanel("Select rom", startingDirectory, string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                GameScriptableObject.Directory = GetRelativePath(Path.GetDirectoryName(filePath)).Replace("\\", "/");
                GameScriptableObject.Name = Path.GetFileNameWithoutExtension(filePath);
            }
        }
    }
}
