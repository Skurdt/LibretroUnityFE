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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UIMenu : MonoBehaviour
    {
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private UIMenuEntryStateSlot _stateSlotMenuEntry;
        [SerializeField] private Button _saveStateButton;
        [SerializeField] private Button _loadStateButton;
        [SerializeField] private Button _saveSRAMButton;
        [SerializeField] private Button _loadSRAMButton;
        [SerializeField] private UIMenuEntryDiskIndex _diskIndexMenuEntry;
        [SerializeField] private Button _replaceDiskButton;
        [SerializeField] private GameObject _toggleFastForwardGameObject;
        [SerializeField] private Toggle _toggleFastForwardToggle;
        [SerializeField] private GameObject _toggleRewindGameObject;
        [SerializeField] private Toggle _toggleRewindToggle;
        [SerializeField] private Button _rewindHoldButton;
        [SerializeField] private Button _coreOptionsOpenButton;
        [SerializeField] private Button _coreOptionsCloseButton;

        private IEnumerable<GameObject> _elementsToEnableOnGameStart;

        private void Awake() => _elementsToEnableOnGameStart = new GameObject[]
        {
            _pauseButton.gameObject,
            _resumeButton.gameObject,
            _stopButton.gameObject,
            _stateSlotMenuEntry.gameObject,
            _saveStateButton.gameObject,
            _loadStateButton.gameObject,
            _saveSRAMButton.gameObject,
            _loadSRAMButton.gameObject,
            _diskIndexMenuEntry.gameObject,
            _replaceDiskButton.gameObject,
            _toggleFastForwardGameObject,
            _toggleRewindGameObject,
            _rewindHoldButton.gameObject,
            _coreOptionsOpenButton.gameObject,
            _coreOptionsCloseButton.gameObject
        };

        private void OnEnable()
        {
            bool validInstance = _uiRoot.Libretro.Current;
            if (validInstance)
            {
                _uiRoot.Libretro.Current.OnInstanceStarted += _uiRoot.LibretroInstanceStartedCallback;
                _uiRoot.Libretro.Current.OnInstanceStopped += _uiRoot.LibretroInstanceStoppedCallback;
            }
            bool runningInstance = validInstance && _uiRoot.Libretro.Current.Running;
            _startButton.interactable = !runningInstance;
            foreach (GameObject menuEntry in _elementsToEnableOnGameStart)
                menuEntry.SetActive(runningInstance);

            _startButton.onClick.AddListener(() => _uiRoot.Libretro.Current.StartContent());
            _pauseButton.onClick.AddListener(() => _uiRoot.Libretro.Current.PauseContent());
            _resumeButton.onClick.AddListener(() => _uiRoot.Libretro.Current.ResumeContent());
            _stopButton.onClick.AddListener(() => _uiRoot.Libretro.Current.StopContent());
            _saveStateButton.onClick.AddListener(() => _uiRoot.Libretro.Current.SaveStateWithScreenshot());
            _loadStateButton.onClick.AddListener(() => _uiRoot.Libretro.Current.LoadState());
            _saveSRAMButton.onClick.AddListener(() => _uiRoot.Libretro.Current.SaveSRAM());
            _loadSRAMButton.onClick.AddListener(() => _uiRoot.Libretro.Current.LoadSRAM());
            _diskIndexMenuEntry.NumImages = _uiRoot.Libretro.Current ? _uiRoot.Libretro.Current.GameNames.Length : 1;
            _replaceDiskButton.onClick.AddListener(() => _uiRoot.Libretro.Current.SetDiskIndex(_diskIndexMenuEntry.CurrentImageIndex));
            _toggleFastForwardToggle.onValueChanged.AddListener((enabled) => _uiRoot.Libretro.Current.FastForward = enabled);
            _toggleRewindToggle.onValueChanged.AddListener((enabled) => _uiRoot.Libretro.Current.Settings.RewindEnabled = enabled);

            EventTrigger eventTrigger = _rewindHoldButton.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTriggerPointerDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            eventTriggerPointerDown.callback.AddListener((eventData) => _uiRoot.Libretro.Current.Rewind = true);
            eventTrigger.triggers.Add(eventTriggerPointerDown);
            EventTrigger.Entry eventTriggerPointerUp = new()
            {
                eventID = EventTriggerType.PointerUp
            };
            eventTriggerPointerUp.callback.AddListener((eventData) => _uiRoot.Libretro.Current.Rewind = false);
            eventTrigger.triggers.Add(eventTriggerPointerUp);

            _coreOptionsOpenButton.onClick.AddListener(() =>
            {
                _uiRoot.Libretro.Current.PauseContent();
                _uiRoot.CoreOptionsOverlay.SetActive(true);
            });

            _coreOptionsCloseButton.onClick.AddListener(() =>
            {
                _uiRoot.Libretro.Current.ResumeContent();
                _uiRoot.CoreOptionsOverlay.SetActive(false);
            });
        }

        private void OnDisable()
        {
            bool validInstance = _uiRoot.Libretro.Current;
            if (validInstance)
            {
                _uiRoot.Libretro.Current.OnInstanceStarted -= _uiRoot.LibretroInstanceStartedCallback;
                _uiRoot.Libretro.Current.OnInstanceStopped -= _uiRoot.LibretroInstanceStoppedCallback;
            }
            _startButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.RemoveAllListeners();
            _resumeButton.onClick.RemoveAllListeners();
            _stopButton.onClick.RemoveAllListeners();
            _saveStateButton.onClick.RemoveAllListeners();
            _loadStateButton.onClick.RemoveAllListeners();
            _saveSRAMButton.onClick.RemoveAllListeners();
            _loadSRAMButton.onClick.RemoveAllListeners();
            _replaceDiskButton.onClick.RemoveAllListeners();
            _toggleFastForwardToggle.onValueChanged.RemoveAllListeners();
            _toggleRewindToggle.onValueChanged.RemoveAllListeners();
            _coreOptionsOpenButton.onClick.RemoveAllListeners();
            _coreOptionsCloseButton.onClick.RemoveAllListeners();
        }

        public void LibretroInstanceStartedCallback()
        {
            _startButton.interactable = false;
            foreach (GameObject menuEntry in _elementsToEnableOnGameStart)
                menuEntry.SetActive(true);
        }

        public void LibretroInstanceStoppedCallback()
        {
            _startButton.interactable = true;
            foreach (GameObject menuEntry in _elementsToEnableOnGameStart)
                menuEntry.SetActive(false);
        }
    }
}
