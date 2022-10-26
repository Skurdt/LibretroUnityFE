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
        private SerializedProperty _libretroInstanceVariableProperty;
        private SerializedProperty _toolbarProperty;
        private SerializedProperty _gameButtonProperty;
        private SerializedProperty _gameMenuProperty;
        private SerializedProperty _gameStartButtonProperty;
        private SerializedProperty _gameResetButtonProperty;
        private SerializedProperty _gameStopButtonProperty;
        private SerializedProperty _stateButtonProperty;
        private SerializedProperty _stateMenuProperty;
        private SerializedProperty _stateSaveButtonProperty;
        private SerializedProperty _stateLoadButtonProperty;

        public override VisualElement CreateInspectorGUI()
        {
            GetProperties();
            return BuildView();
        }

        private void GetProperties()
        {
            _libretroInstanceVariableProperty = serializedObject.FindProperty($"<{nameof(UI_Root.Libretro)}>k__BackingField");
            _toolbarProperty                  = serializedObject.FindProperty($"<{nameof(UI_Root.Toolbar)}>k__BackingField");
            _gameButtonProperty               = serializedObject.FindProperty($"<{nameof(UI_Root.GameButton)}>k__BackingField");
            _gameMenuProperty                 = serializedObject.FindProperty($"<{nameof(UI_Root.GameMenu)}>k__BackingField");
            _gameStartButtonProperty          = serializedObject.FindProperty($"<{nameof(UI_Root.GameStartButton)}>k__BackingField");
            _gameResetButtonProperty          = serializedObject.FindProperty($"<{nameof(UI_Root.GameResetButton)}>k__BackingField");
            _gameStopButtonProperty           = serializedObject.FindProperty($"<{nameof(UI_Root.GameStopButton)}>k__BackingField");
            _stateButtonProperty              = serializedObject.FindProperty($"<{nameof(UI_Root.StateButton)}>k__BackingField");
            _stateMenuProperty                = serializedObject.FindProperty($"<{nameof(UI_Root.StateMenu)}>k__BackingField");
            _stateSaveButtonProperty          = serializedObject.FindProperty($"<{nameof(UI_Root.StateSaveButton)}>k__BackingField");
            _stateLoadButtonProperty          = serializedObject.FindProperty($"<{nameof(UI_Root.StateLoadButton)}>k__BackingField");
        }

        private VisualElement BuildView()
        {
            VisualElement root = new();

            PropertyField scriptPropertyField = new(serializedObject.FindProperty("m_Script"));
            scriptPropertyField.SetEnabled(false);
            root.Add(scriptPropertyField);

            root.Add(new PropertyField(_libretroInstanceVariableProperty));

            VisualElement toolbarElement = new();
            root.Add(toolbarElement);
            toolbarElement.Add(new PropertyField(_toolbarProperty));

            ToolbarAddGameMenu(toolbarElement);
            ToolbarAddStateMenu(toolbarElement);

            return root;
        }

        private void ToolbarAddGameMenu(VisualElement toolbar)
        {
            VisualElement button = PaddedPropertyField(_gameButtonProperty, toolbar);
            VisualElement menu   = PaddedPropertyField(_gameMenuProperty, button);
            _ = PaddedPropertyField(_gameStartButtonProperty, menu);
            _ = PaddedPropertyField(_gameResetButtonProperty, menu);
            _ = PaddedPropertyField(_gameStopButtonProperty, menu);
        }

        private void ToolbarAddStateMenu(VisualElement toolbar)
        {
            VisualElement button = PaddedPropertyField(_stateButtonProperty, toolbar);
            VisualElement menu   = PaddedPropertyField(_stateMenuProperty, button);
            _ = PaddedPropertyField(_stateSaveButtonProperty, menu);
            _ = PaddedPropertyField(_stateLoadButtonProperty, menu);
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
