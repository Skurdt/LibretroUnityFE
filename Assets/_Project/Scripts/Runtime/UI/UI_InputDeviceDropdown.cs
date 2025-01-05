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
        private int _index;

        private void OnEnable() => _dropdown.onValueChanged.AddListener((index) =>
        {
            if (_libretro)
                _libretro.SetControllerPortDevice((uint)_index, (RETRO_DEVICE)index);
        });

        private void OnDisable() => _dropdown.onValueChanged.RemoveAllListeners();

        public void Init(LibretroInstance libretro, int index, RETRO_DEVICE device)
        {
            if (!libretro)
                return;

            _libretro = libretro;
            _index    = index;

            _dropdown.ClearOptions();

            _label.SetText($"Player{_index}");

            _dropdown.AddOptions(Enum.GetNames(typeof(RETRO_DEVICE)).ToList());
            int valueIndex = _dropdown.options.FindIndex(x => x.text.Equals(device.ToString(), StringComparison.OrdinalIgnoreCase));
            _dropdown.SetValueWithoutNotify(valueIndex);
        }
    }
}
