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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UI_CoreOptionsMenu : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _listContent;
        [SerializeField] private UICoreOptionDropdown _dropdownTemplatePrefab;

        private readonly List<GameObject> _instantiatedObjects = new();

        private LibretroInstanceVariable _libretro;

        private void OnEnable()
        {
            GenerateOptionsList();
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => SetVisible(false));
        }

        private void OnDisable()
        {
            ClearOptionsList();
            _closeButton.onClick.RemoveAllListeners();
        }

        public void Construct(LibretroInstanceVariable libretro)
        {
            _libretro = libretro;
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            _libretro.SetInputEnabled(!visible);
        }

        private void GenerateOptionsList()
        {
            ClearOptionsList();

            if (!_libretro.Current)
                return;

            (Options coreOptions, Options gameOptions) = _libretro.Current.Options;

            for (int i = 0; i < coreOptions.Count; ++i)
            {
                if (!coreOptions[i].Visible)
                    continue;

                UICoreOptionDropdown optionInstance = Instantiate(_dropdownTemplatePrefab, _listContent);
                optionInstance.Init(_libretro.Current, coreOptions[i], gameOptions?[i]);
                _instantiatedObjects.Add(optionInstance.gameObject);
            }
        }

        private void ClearOptionsList()
        {
            for (int i = _instantiatedObjects.Count - 1; i >= 0; --i)
                Destroy(_instantiatedObjects[i]);
            _instantiatedObjects.Clear();
        }
    }
}
