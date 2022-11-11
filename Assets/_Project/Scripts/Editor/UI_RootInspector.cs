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
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SK.Libretro.Examples.Editor
{
    [CustomEditor(typeof(UI_Root))]
    internal sealed class UI_RootInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            PropertyField scriptPropertyField = new(serializedObject.FindProperty("m_Script"));
            scriptPropertyField.SetEnabled(false);
            root.Add(scriptPropertyField);

            root.Add(new PropertyField(serializedObject.FindProperty("_libretro")));

            VisualElement toolbarElement = new();
            root.Add(toolbarElement);
            toolbarElement.Add(new PropertyField(serializedObject.FindProperty("_toolbar")));

            ToolbarAddMenu("_game", toolbarElement);
            ToolbarAddMenu("_state", toolbarElement);
            ToolbarAddMenu("_disk", toolbarElement);
            ToolbarAddMenu("_memory", toolbarElement);
            VisualElement coreOptionsButton = PaddedPropertyField(serializedObject.FindProperty("_coreOptionsButton"), toolbarElement);
            _ = PaddedPropertyField(serializedObject.FindProperty("_coreOptionsMenu"), coreOptionsButton);

            return root;
        }

        private void ToolbarAddMenu(string id, VisualElement toolbar)
        {
            VisualElement button = PaddedPropertyField(serializedObject.FindProperty($"{id}Button"), toolbar);
            SerializedProperty menuProperty = serializedObject.FindProperty($"{id}Menu");
            VisualElement menu = PaddedPropertyField(menuProperty, button);
            while (menuProperty.Next(true))
                if (menuProperty.name.StartsWith(id))
                    _ = PaddedPropertyField(menuProperty, menu);
        }

        private static VisualElement PaddedPropertyField(SerializedProperty serializedProperty, VisualElement parent)
        {
            VisualElement element = new();
            element.style.paddingLeft = 15f;
            element.Add(new PropertyField(serializedProperty));
            parent.Add(element);
            return element;
        }
    }
}
