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

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using static SK.Libretro.Utilities.FileSystem;

namespace SK.Examples.Common
{
    [CustomEditor(typeof(GameModelSetup))]
    public class GameModelSetupInspector : Editor
    {
        public GameModelSetup ModelSetupScript { get; private set; }

        private void OnEnable() => ModelSetupScript = target as GameModelSetup;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (ModelSetupScript.Game != null)
            {
                GUILayout.Space(8f);

                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Core", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    ShowSelectCoreWindow();
                }
                ModelSetupScript.Game.Core = EditorGUILayout.TextField(ModelSetupScript.Game.Core);
                EditorGUILayout.EndHorizontal();

                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Directory", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    ShowSelectRomDirectoryDialog();
                }
                ModelSetupScript.Game.Directory = EditorGUILayout.TextField(ModelSetupScript.Game.Directory);
                EditorGUILayout.EndHorizontal();

                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Rom", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    ShowSelectRomDialog();
                }
                ModelSetupScript.Game.Name = EditorGUILayout.TextField(ModelSetupScript.Game.Name);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(8f);

                if (!string.IsNullOrEmpty(ModelSetupScript.Game.Core))
                {
                    _ = EditorGUILayout.BeginHorizontal();
                    if (!EditorApplication.isPlaying)
                    {
                        if (GUILayout.Button("Start", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
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
        }

        private void ShowSelectCoreWindow()
        {
            string startingDirectory = GetAbsolutePath($"{Application.streamingAssetsPath}/libretro~/cores");
            string filePath          = EditorUtility.OpenFilePanelWithFilters("Select core", startingDirectory, new string[] { "Libretro Core", "dll,so,dylib" });
            if (!string.IsNullOrEmpty(filePath))
            {
                ModelSetupScript.Game.Core = Path.GetFileNameWithoutExtension(filePath).Replace("_libretro", string.Empty);
            }
        }

        private void ShowSelectRomDirectoryDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(ModelSetupScript.Game.Directory))
            {
                startingDirectory = GetAbsolutePath(ModelSetupScript.Game.Directory);
            }

            string directory = EditorUtility.OpenFolderPanel("Select rom directory", startingDirectory, startingDirectory);
            if (!string.IsNullOrEmpty(directory))
            {
                ModelSetupScript.Game.Directory = GetRelativePath(directory).Replace(Path.DirectorySeparatorChar, '/');
            }
        }

        private void ShowSelectRomDialog()
        {
            string startingDirectory = string.Empty;
            if (!string.IsNullOrEmpty(ModelSetupScript.Game.Directory))
            {
                startingDirectory = GetAbsolutePath(ModelSetupScript.Game.Directory);
            }

            string filePath = EditorUtility.OpenFilePanel("Select rom", startingDirectory, string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                ModelSetupScript.Game.Directory = GetRelativePath(Path.GetDirectoryName(filePath)).Replace(Path.DirectorySeparatorChar, '/');
                ModelSetupScript.Game.Name = Path.GetFileNameWithoutExtension(filePath);
            }
        }
    }
}
#endif
