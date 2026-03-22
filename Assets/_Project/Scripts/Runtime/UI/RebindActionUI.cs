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

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SK.Libretro.Examples
{
    public class RebindActionUI : MonoBehaviour
    {
        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }

        [SerializeField] private InputActionReference _action;
        [SerializeField] private string _bindingId;
        [SerializeField] private InputBinding.DisplayStringOptions _displayStringOptions;
        [SerializeField] private TMP_Text _actionLabel;
        [SerializeField] private TMP_Text _bindingText;
        [field:SerializeField] public GameObject RebindOverlay { get; private set; }
        [field: SerializeField] public TMP_Text RebindText { get; private set; }
        [SerializeField] private UpdateBindingUIEvent _updateBindingUIEvent;
        [SerializeField] private InteractiveRebindEvent _rebindStartEvent;
        [SerializeField] private InteractiveRebindEvent _rebindStopEvent;

        public InputActionReference ActionReference
        {
            get => _action;
            set
            {
                _action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        public string BindingId
        {
            get => _bindingId;
            set
            {
                _bindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions DisplayStringOptions
        {
            get => _displayStringOptions;
            set
            {
                _displayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        public TMP_Text ActionLabel
        {
            get => _actionLabel;
            set
            {
                _actionLabel = value;
                UpdateActionLabel();
            }
        }

        public TMP_Text BindingText
        {
            get => _bindingText;
            set
            {
                _bindingText = value;
                UpdateBindingDisplay();
            }
        }

        public UpdateBindingUIEvent OnUpdateBindingUI
        {
            get
            {
                _updateBindingUIEvent ??= new UpdateBindingUIEvent();
                return _updateBindingUIEvent;
            }
        }

        public InteractiveRebindEvent StartRebindEvent
        {
            get
            {
                _rebindStartEvent ??= new InteractiveRebindEvent();
                return _rebindStartEvent;
            }
        }

        public InteractiveRebindEvent StopRebindEvent
        {
            get
            {
                _rebindStopEvent ??= new InteractiveRebindEvent();
                return _rebindStopEvent;
            }
        }

        public InputActionRebindingExtensions.RebindingOperation OngoingRebind { get; private set; }

        private static List<RebindActionUI> _rebindActionUIs;

        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = _action != null ? _action.action : null;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(_bindingId))
                return false;

            Guid bindingId = new(_bindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            string deviceLayoutName = default;
            string controlPath = default;

            var action = _action != null ? _action.action : null;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == _bindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, DisplayStringOptions);
            }

            if (_bindingText != null)
                _bindingText.text = displayString;

            _updateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }
            UpdateBindingDisplay();
        }

        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            OngoingRebind?.Cancel();

            void CleanUp()
            {
                OngoingRebind?.Dispose();
                OngoingRebind = null;
            }

            OngoingRebind = action.PerformInteractiveRebinding(bindingIndex)
                .WithTimeout(5f)
                .OnCancel(
                    operation =>
                    {
                        _rebindStopEvent?.Invoke(this, operation);
                        if (RebindOverlay != null)
                            RebindOverlay.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        if (RebindOverlay != null)
                            RebindOverlay.SetActive(false);
                        _rebindStopEvent?.Invoke(this, operation);
                        UpdateBindingDisplay();
                        CleanUp();

                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            string partName = default;
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

            if (RebindOverlay != null)
                RebindOverlay.SetActive(true);
            if (RebindText != null)
            {
                var text = !string.IsNullOrEmpty(OngoingRebind.expectedControlType)
                    ? $"{partName}Waiting for {OngoingRebind.expectedControlType} input..."
                    : $"{partName}Waiting for input...";
                RebindText.text = text;
            }

            if (RebindOverlay == null && RebindText == null && _rebindStartEvent == null && _bindingText != null)
                _bindingText.text = "<Waiting...>";

            _rebindStartEvent?.Invoke(this, OngoingRebind);

            _ = OngoingRebind.Start();
        }

        protected void OnEnable()
        {
            _rebindActionUIs ??= new List<RebindActionUI>();
            _rebindActionUIs.Add(this);
            if (_rebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;

            UpdateBindingDisplay();
        }

        protected void OnDisable()
        {
            OngoingRebind?.Dispose();
            OngoingRebind = null;

            _ = _rebindActionUIs.Remove(this);
            if (_rebindActionUIs.Count == 0)
            {
                _rebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action != null ? action.actionMap ?? obj as InputActionMap : null;
            var actionAsset = actionMap?.asset != null ? obj as InputActionAsset : null;

            for (var i = 0; i < _rebindActionUIs.Count; ++i)
            {
                var component = _rebindActionUIs[i];
                var referencedAction = component.ActionReference != null ? component.ActionReference.action : null;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
#endif

        private void UpdateActionLabel()
        {
            if (_actionLabel != null)
            {
                var action = _action != null ? _action.action : null;
                _actionLabel.text  = action != null ? action.name : string.Empty;
            }
        }
    }
}
