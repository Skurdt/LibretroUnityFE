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

namespace SK.Examples
{
    public sealed class UICoreOptionDropdown : UICoreOption
    {
        [SerializeField] private TMP_Dropdown _dropdown;

        private string _coreName;
        private CoreOption _option;

        private void OnEnable() => _dropdown.onValueChanged.AddListener((index) => CoreInstances.Instance.UpdateCoreOptionValue(_coreName, _option.Key, index, true));

        private void OnDisable() => _dropdown.onValueChanged.RemoveAllListeners();

        public void Init(string coreName, CoreOption option)
        {
            _coreName = coreName;
            _option   = option;
            _label.SetText(option.Description);
            _dropdown.AddOptions(option.PossibleValues.ToList());
            int valueIndex = _dropdown.options.FindIndex(x => x.text.Equals(option.CurrentValue, System.StringComparison.OrdinalIgnoreCase));
            _dropdown.SetValueWithoutNotify(valueIndex);
        }
    }
}
