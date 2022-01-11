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

using SK.Libretro;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class UICoreOptionDropdown : UICoreOption
    {
        [SerializeField] private TMP_Dropdown _dropdownCore;
        [SerializeField] private TMP_Dropdown _dropdownGame;

        private string _coreName;
        private CoreOption _optionCore;
        private string _gameValue;

        private void OnEnable()
        {
            _dropdownCore.onValueChanged.AddListener((index) => CoreInstances.Instance.UpdateCoreOptionValue(_coreName, _optionCore?.Key, index, true));
            _dropdownGame.onValueChanged.AddListener((index) => CoreInstances.Instance.UpdateCoreOptionValue(_coreName, _optionCore?.Key, index, false));
        }

        private void OnDisable()
        {
            _dropdownCore.onValueChanged.RemoveAllListeners();
            _dropdownGame.onValueChanged.RemoveAllListeners();
        }

        public void Init(string coreName, CoreOption optionCore, string gameValue)
        {
            _coreName   = coreName;
            _optionCore = optionCore;
            if (optionCore != null)
            {
                _dropdownCore.ClearOptions();
                _dropdownGame.ClearOptions();

                _label.SetText(optionCore.Description);

                _dropdownCore.AddOptions(optionCore.PossibleValues.ToList());
                _dropdownGame.AddOptions(optionCore.PossibleValues.ToList());

                int valueIndex = _dropdownCore.options.FindIndex(x => x.text.Equals(optionCore.CurrentValue, System.StringComparison.OrdinalIgnoreCase));
                _dropdownCore.SetValueWithoutNotify(valueIndex);

                if (!string.IsNullOrWhiteSpace(gameValue))
                {
                    valueIndex = _dropdownCore.options.FindIndex(x => x.text.Equals(gameValue, System.StringComparison.OrdinalIgnoreCase));
                    _dropdownGame.SetValueWithoutNotify(valueIndex);
                }
            }
        }
    }
}
