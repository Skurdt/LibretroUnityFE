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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace SK.Examples.Common
{
    [CustomEditor(typeof(GameModelSetup), true)]
    public sealed class GameModelSetupInspector : Editor
    {
        private GameModelSetup _gameModelSetup;

        private void OnEnable()
        {
            _gameModelSetup = target as GameModelSetup;

            if (_gameModelSetup != null)
            {
                EditorGUI.FocusTextInControl(null);
                _gameModelSetup.LoadConfig();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(8f);

            if (_gameModelSetup == null)
            {
                EditorGUILayout.HelpBox("GameModelSetup is null", MessageType.Error);
                return;
            }

            using (new EditorGUI.DisabledGroupScope(EditorApplication.isPlaying))
            {
                if (GUILayout.Button("Load", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
                {
                    EditorGUI.FocusTextInControl(null);
                    _gameModelSetup.LoadConfig();
                }

                GUILayout.Space(8f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Core", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        ShowSelectCoreWindow();
                    _gameModelSetup.CoreName = EditorGUILayout.TextField(_gameModelSetup.CoreName);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Directory", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        ShowSelectRomDirectoryDialog();
                    _gameModelSetup.GameDirectory = EditorGUILayout.TextField(_gameModelSetup.GameDirectory);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Rom", GUILayout.Width(100f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        ShowSelectRomDialog();
                    _gameModelSetup.GameName = EditorGUILayout.TextField(_gameModelSetup.GameName);
                }

                GUILayout.Space(8f);

                if (!string.IsNullOrEmpty(_gameModelSetup.CoreName))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Save", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
                            _gameModelSetup.SaveConfig();
                    }
                }
                else
                    EditorGUILayout.HelpBox("No core selected", MessageType.Error);
            }
        }

        private void ShowSelectCoreWindow()
        {
            string startingDirectory = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "libretro~/cores"));
            string filePath          = EditorUtility.OpenFilePanelWithFilters("Select core", startingDirectory, new string[] { "Libretro Core", "dll,so,dylib" });
            if (!string.IsNullOrEmpty(filePath))
                _gameModelSetup.CoreName = Path.GetFileNameWithoutExtension(filePath).Replace("_libretro", "");
        }

        private void ShowSelectRomDirectoryDialog()
        {
            string startingDirectory = !string.IsNullOrEmpty(_gameModelSetup.GameDirectory) ? Path.GetFullPath(_gameModelSetup.GameDirectory) : "";
            string directory         = EditorUtility.OpenFolderPanel("Select rom directory", startingDirectory, startingDirectory);
            if (!string.IsNullOrEmpty(directory))
                _gameModelSetup.GameDirectory = Path.GetFullPath(directory).Replace(Path.DirectorySeparatorChar, '/');
        }

        private void ShowSelectRomDialog()
        {
            string startingDirectory = !string.IsNullOrEmpty(_gameModelSetup.GameDirectory) ? Path.GetFullPath(_gameModelSetup.GameDirectory) : "";
            string filePath          = EditorUtility.OpenFilePanel("Select rom", startingDirectory, "");
            if (!string.IsNullOrEmpty(filePath))
            {
                _gameModelSetup.GameDirectory = Path.GetFullPath(Path.GetDirectoryName(filePath)).Replace(Path.DirectorySeparatorChar, '/');
                _gameModelSetup.GameName      = Path.GetFileNameWithoutExtension(filePath);
            }
        }
    }
}
