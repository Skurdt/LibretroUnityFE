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

using SK.Libretro.Header;
using SK.Libretro.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class UI_InputDeviceDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Dropdown _dropdown;

        private LibretroInstance _libretro;
        private int _port;

        private void OnEnable() => _dropdown.onValueChanged.AddListener((device) =>
        {
            if (!_libretro)
                return;

            if (_libretro.ControllersMap[_port] is null)
            {
                _libretro.SetControllerPortDevice((uint)_port, (uint)device);
                return;
            }

            Controllers controllers = _libretro.ControllersMap[_port];
            Controller controller = controllers.FirstOrDefault(x => x.Description.Equals(_dropdown.options[device].text, StringComparison.OrdinalIgnoreCase));
            if (controller is not null)
                _libretro.SetControllerPortDevice((uint)_port, controller.Device);
        });

        private void OnDisable() => _dropdown.onValueChanged.RemoveAllListeners();

        public void Init(LibretroInstance libretro, int port, uint device)
        {
            if (!libretro)
                return;

            _libretro = libretro;
            _port     = port;

            _label.SetText($"Player{_port}");

            _dropdown.ClearOptions();
            int currentValueIndex;

            Controllers controllers = _libretro.ControllersMap[port];
            if (controllers is null)
            {
                _dropdown.AddOptions(Enum.GetNames(typeof(RETRO_DEVICE)).ToList());
                currentValueIndex = _dropdown.options.FindIndex(x => x.text.Equals(device.ToString(), StringComparison.OrdinalIgnoreCase));
                _dropdown.SetValueWithoutNotify(currentValueIndex);
                return;
            }

            List<string> dropdownChoices = controllers.Select(x => x.Description).ToList();
            _dropdown.AddOptions(dropdownChoices);
            string currentValueString = controllers.FirstOrDefault(x => x.Device == device)?.Description;
            currentValueIndex = _dropdown.options.FindIndex(x => x.text.Equals(currentValueString, StringComparison.OrdinalIgnoreCase));
            _dropdown.SetValueWithoutNotify(currentValueIndex);
        }
    }
}
