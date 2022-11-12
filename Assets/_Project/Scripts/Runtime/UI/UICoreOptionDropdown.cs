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
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class UICoreOptionDropdown : UICoreOption
    {
        [SerializeField] private TMP_Dropdown _dropdownCore;
        [SerializeField] private TMP_Dropdown _dropdownGame;

        private LibretroInstance _libretro;
        private Option _coreOption;
        private Option _gameOption;

        private void OnEnable()
        {
            _dropdownCore.onValueChanged.AddListener((index) =>
            {
                if (_coreOption is null)
                    return;

                string previousValue = _coreOption.CurrentValue;
                _coreOption.Update(index);
                if (_gameOption is not null && _gameOption.CurrentValue.Equals(previousValue, StringComparison.OrdinalIgnoreCase))
                {
                    _dropdownGame.SetValueWithoutNotify(index);
                    _gameOption.Update(index);
                }

                if (_libretro)
                    _libretro.SaveOptions(true);
            });
            _dropdownGame.onValueChanged.AddListener((index) =>
            {
                if (_gameOption is null)
                    return;

                _gameOption.Update(index);

                if (_libretro)
                    _libretro.SaveOptions(false);
            });
        }

        private void OnDisable()
        {
            _dropdownCore.onValueChanged.RemoveAllListeners();
            _dropdownGame.onValueChanged.RemoveAllListeners();
        }

        public void Init(LibretroInstance libretro, Option coreOption, Option gameOption)
        {
            if (!libretro)
                return;

            _libretro = libretro;

            _dropdownCore.ClearOptions();
            _dropdownGame.ClearOptions();

            if (coreOption is null)
                return;

            _coreOption = coreOption;
            _label.SetText(coreOption.Description);
            AddOptions(_dropdownCore, coreOption);

            if (gameOption is null)
                return;

            _gameOption = gameOption;
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
