/* MIT License

 * Copyright (c) 2021-2022 Skurdt
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

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SK.Libretro.Examples.Editor
{
    [CustomEditor(typeof(JsonConfigLoader))]
    internal sealed class JsonConfigLoaderInspector : UnityEditor.Editor
    {
        private JsonConfigLoader _jsonConfigLoader;
        private SerializedProperty _instancesParentProperty;

        private void OnEnable()
        {
            _jsonConfigLoader        = target as JsonConfigLoader;
            _instancesParentProperty = serializedObject.FindProperty(nameof(JsonConfigLoader.InstancesParent));
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(8f);
            _ = EditorGUILayout.PropertyField(_instancesParentProperty);

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Load", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
                {
                    EditorGUI.FocusTextInControl(null);

                    Scene activeScene = SceneManager.GetActiveScene();

                    if (!EditorApplication.isPlaying)
                        _ = EditorSceneManager.MarkSceneDirty(activeScene);

                    _jsonConfigLoader.LoadConfig();

                    if (!EditorApplication.isPlaying)
                        _ = EditorSceneManager.SaveScene(activeScene);
                }

                if (GUILayout.Button("Save", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
                {
                    if (!EditorApplication.isPlaying)
                        _ = EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

                    EditorGUI.FocusTextInControl(null);
                    _jsonConfigLoader.SaveConfig();
                }
            }
        }
    }
}
