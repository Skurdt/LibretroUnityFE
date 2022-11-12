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

using System.Linq;
using TMPro;
using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class UICoreOptionDropdown : UICoreOption
    {
        [SerializeField] private TMP_Dropdown _dropdownCore;
        [SerializeField] private TMP_Dropdown _dropdownGame;

        private Option _coreOption;
        private Option _gameOption;

        private void OnEnable()
        {
            _dropdownCore.onValueChanged.AddListener((index) => _coreOption?.Update(index));
            _dropdownGame.onValueChanged.AddListener((index) => _gameOption?.Update(index));
        }

        private void OnDisable()
        {
            _dropdownCore.onValueChanged.RemoveAllListeners();
            _dropdownGame.onValueChanged.RemoveAllListeners();
        }

        public void Init(Option coreOption, Option gameOption)
        {
            _coreOption = coreOption;
            _gameOption = gameOption;

            _dropdownCore.ClearOptions();
            _dropdownGame.ClearOptions();

            if (coreOption is null)
                return;

            _label.SetText(coreOption.Description);
            AddOptions(_dropdownCore, coreOption);

            if (gameOption is not null)
                AddOptions(_dropdownGame, gameOption);
        }

        private static void AddOptions(TMP_Dropdown dropdown, Option option)
        {
            dropdown.AddOptions(option.PossibleValues.ToList());
            int valueIndex = dropdown.options.FindIndex(x => x.text.Equals(option.CurrentValue, System.StringComparison.OrdinalIgnoreCase));
            dropdown.SetValueWithoutNotify(valueIndex);
        }
    }
}
