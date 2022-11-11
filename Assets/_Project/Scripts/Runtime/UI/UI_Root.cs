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

using SK.Libretro.Unity;
using Unity.Mathematics;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, DefaultExecutionOrder(1000)]
    public sealed class UI_Root : MonoBehaviour
    {
        [SerializeField] private LibretroInstanceVariable _libretro;
        [SerializeField] private UI_Toolbar _toolbar;
        [SerializeField] private UI_Button _gameButton;
        [SerializeField] private UI_ToolbarMenu _gameMenu;
        [SerializeField] private UI_Button _gameStartButton;
        [SerializeField] private UI_Button _gameResetButton;
        [SerializeField] private UI_Button _gameStopButton;
        [SerializeField] private UI_Button _stateButton;
        [SerializeField] private UI_ToolbarMenu _stateMenu;
        [SerializeField] private UI_Button _stateDecreaseSlotButton;
        [SerializeField] private UI_Button _stateIncreaseSlotButton;
        [SerializeField] private UI_Button _stateSaveButton;
        [SerializeField] private UI_Button _stateLoadButton;
        [SerializeField] private UI_Button _diskButton;
        [SerializeField] private UI_ToolbarMenu _diskMenu;
        [SerializeField] private UI_Button _diskDecreaseIndexButton;
        [SerializeField] private UI_Button _diskIncreaseIndexButton;
        [SerializeField] private UI_Button _diskReplaceButton;
        [SerializeField] private UI_Button _memoryButton;
        [SerializeField] private UI_ToolbarMenu _memoryMenu;
        [SerializeField] private UI_Button _memorySaveSRAMButton;
        [SerializeField] private UI_Button _memoryLoadSRAMButton;
        [SerializeField] private UI_Button _coreOptionsButton;
        [SerializeField] private UI_CoreOptionsMenu _coreOptionsMenu;

        private int _stateSlot;
        private int _diskIndex;

        private void OnEnable()
        {
            _libretro.OnInstanceChanged += LibretroInstanceChangedCallback;
            LibretroInstanceChangedCallback(_libretro.Current);

            _toolbar.Construct(true, _libretro);

            _gameButton.Construct(true, true, () => _gameMenu.SetVisible(true));
            _gameMenu.Construct(false, _libretro);
            _gameStartButton.Construct(true, true, () => _libretro.StartContent());
            _gameResetButton.Construct(true, false, () => _libretro.ResetContent());
            _gameStopButton.Construct(true, false, () => _libretro.StopContent());

            _stateButton.Construct(true, false, () => _stateMenu.SetVisible(true));
            _stateMenu.Construct(false, _libretro);
            _stateDecreaseSlotButton.Construct(true, false, () =>
            {
                _stateSlot = math.max(0, --_stateSlot);
                _stateSaveButton.Text = $"Save ({_stateSlot})";
                _stateLoadButton.Text = $"Load ({_stateSlot})";
                _stateDecreaseSlotButton.SetInteractable(_stateSlot > 0);
                _stateIncreaseSlotButton.SetInteractable(_stateSlot < 999999);
            });
            _stateIncreaseSlotButton.Construct(true, true, () =>
            {
                _stateSlot = math.min(++_stateSlot, 999999);
                _stateSaveButton.Text = $"Save ({_stateSlot})";
                _stateLoadButton.Text = $"Load ({_stateSlot})";
                _stateDecreaseSlotButton.SetInteractable(_stateSlot > 0);
                _stateIncreaseSlotButton.SetInteractable(_stateSlot < 999999);
            });
            _stateSaveButton.Construct(true, true, () => _libretro.SaveState(_stateSlot));
            _stateLoadButton.Construct(true, true, () => _libretro.LoadState(_stateSlot));

            _diskButton.Construct(true, false, () => _diskMenu.SetVisible(true));
            _diskMenu.Construct(false, _libretro);
            _diskDecreaseIndexButton.Construct(true, false, () =>
            {
                _diskIndex = math.max(0, --_diskIndex);
                _diskReplaceButton.Text = $"Replace ({_diskIndex})";
                _diskDecreaseIndexButton.SetInteractable(_diskIndex > 0);
                _diskIncreaseIndexButton.SetInteractable(_diskIndex < 999999);
            });
            _diskIncreaseIndexButton.Construct(true, true, () =>
            {
                _diskIndex = math.min(++_diskIndex, 999999);
                _diskReplaceButton.Text = $"Replace ({_diskIndex})";
                _diskDecreaseIndexButton.SetInteractable(_diskIndex > 0);
                _diskIncreaseIndexButton.SetInteractable(_diskIndex < 999999);
            });
            _diskReplaceButton.Construct(true, true, () => _libretro.SetDiskIndex(_diskIndex));

            _memoryButton.Construct(true, false, () => _memoryMenu.SetVisible(true));
            _memoryMenu.Construct(false, _libretro);
            _memorySaveSRAMButton.Construct(true, true, () => _libretro.SaveSRAM());
            _memoryLoadSRAMButton.Construct(true, true, () => _libretro.LoadSRAM());

            _coreOptionsMenu.Construct(_libretro);
            _coreOptionsButton.Construct(true, false, () => _coreOptionsMenu.SetVisible(true));
        }

        private void OnDisable()
        {
            _libretro.OnInstanceChanged -= LibretroInstanceChangedCallback;
            if (_libretro.Current)
            {
                _libretro.Current.OnInstanceStarted -= LibretronInstanceStartedCallback;
                _libretro.Current.OnInstanceStopped -= LibretronInstanceStoppedCallback;
            }
        }

        private void LibretroInstanceChangedCallback(LibretroInstance libretroInstance)
        {
            if (!libretroInstance)
                return;

            libretroInstance.OnInstanceStarted -= LibretronInstanceStartedCallback;
            libretroInstance.OnInstanceStarted += LibretronInstanceStartedCallback;
            libretroInstance.OnInstanceStopped -= LibretronInstanceStoppedCallback;
            libretroInstance.OnInstanceStopped += LibretronInstanceStoppedCallback;
        }

        private void LibretronInstanceStartedCallback()
        {
            _gameStartButton.Text = "Pause";
            _gameStartButton.SetCallback(() =>
            {
                _libretro.PauseContent();
                _gameStartButton.Text = "Resume";
                _gameStartButton.SetCallback(() =>
                {
                    _libretro.ResumeContent();
                    _gameStartButton.Text = "Pause";
                });
            });

            _gameStopButton.SetInteractable(true);

            _stateButton.SetInteractable(true);
            _diskButton.SetInteractable(_libretro.DiskHandlerEnabled);
            _memoryButton.SetInteractable(true);
            _coreOptionsButton.SetInteractable(true);
        }

        private void LibretronInstanceStoppedCallback()
        {
            _gameStartButton.Text = "Start";
            _gameStartButton.SetCallback(() => _libretro.StartContent());

            _gameStopButton.SetInteractable(false);

            _stateButton.SetInteractable(false);
            _diskButton.SetInteractable(false);
            _memoryButton.SetInteractable(false);
            _coreOptionsButton.SetInteractable(false);
        }
    }
}
