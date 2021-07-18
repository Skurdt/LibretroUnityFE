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

using SK.Libretro.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SK.Examples
{
    public sealed class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference _leftClickAction;
        [SerializeField] private LibretroInstance _libretroInstance;

        [SerializeField] private Button _startButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private UIMainMenuStateSlot _uiMainMenuStateSlot;
        [SerializeField] private Button _saveStateButton;
        [SerializeField] private Button _loadStateButton;
        [SerializeField] private Button _saveSRAMButton;
        [SerializeField] private Button _loadSRAMButton;
        [SerializeField] private Toggle _fastForwardToggle;
        [SerializeField] private Toggle _rewindEnabledToggle;
        [SerializeField] private Button _rewindHoldButton;
        [SerializeField] private Button _coreOptionsButton;
        [SerializeField] private GameObject _coreOptionsPanel;

        private void Awake()
        {
            if (_uiMainMenuStateSlot == null)
                _uiMainMenuStateSlot = GetComponentInChildren<UIMainMenuStateSlot>(true);
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(() => _libretroInstance.StartContent());
            _pauseButton.onClick.AddListener(() => _libretroInstance.PauseContent());
            _resumeButton.onClick.AddListener(() => _libretroInstance.ResumeContent());
            _stopButton.onClick.AddListener(() => _libretroInstance.StopContent());
            _saveStateButton.onClick.AddListener(() => _libretroInstance.SaveStateWithScreenshot());
            _loadStateButton.onClick.AddListener(() => _libretroInstance.LoadState());
            _saveSRAMButton.onClick.AddListener(() => _libretroInstance.SaveSRAM());
            _loadSRAMButton.onClick.AddListener(() => _libretroInstance.LoadSRAM());
            _fastForwardToggle.onValueChanged.AddListener((enabled) => _libretroInstance.FastForwarding = enabled);
            _rewindEnabledToggle.onValueChanged.AddListener((enabled) => _libretroInstance.Settings.RewindEnabled = enabled);

            EventTrigger eventTrigger = _rewindHoldButton.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTriggerPointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            eventTriggerPointerDown.callback.AddListener((eventData) => _libretroInstance.PerformRewind = true);
            eventTrigger.triggers.Add(eventTriggerPointerDown);
            EventTrigger.Entry eventTriggerPointerUp = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            eventTriggerPointerUp.callback.AddListener((eventData) => _libretroInstance.PerformRewind = false);
            eventTrigger.triggers.Add(eventTriggerPointerUp);

            _coreOptionsButton.onClick.AddListener(() => _coreOptionsPanel.SetActive(true));
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.RemoveAllListeners();
            _resumeButton.onClick.RemoveAllListeners();
            _stopButton.onClick.RemoveAllListeners();
            _saveStateButton.onClick.RemoveAllListeners();
            _loadStateButton.onClick.RemoveAllListeners();
            _saveSRAMButton.onClick.RemoveAllListeners();
            _loadSRAMButton.onClick.RemoveAllListeners();
            _fastForwardToggle.onValueChanged.RemoveAllListeners();
            _rewindEnabledToggle.onValueChanged.RemoveAllListeners();
            _coreOptionsButton.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (_leftClickAction == null)
                return;

            bool mouseOverUI      = IsPointerOverUIObject();
            bool leftClickEnabled = _leftClickAction.action.enabled;
            if (mouseOverUI && leftClickEnabled)
                _leftClickAction.action.Disable();
            else if (!mouseOverUI && !leftClickEnabled)
                _leftClickAction.action.Enable();
        }

        public void SetLibretroInstance(LibretroInstance libtetroInstance) => _libretroInstance = libtetroInstance;

        public void SetStateSlot(int slot) => _libretroInstance.SetStateSlot(slot);

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Any(x => x.gameObject == gameObject);
        }
    }
}
