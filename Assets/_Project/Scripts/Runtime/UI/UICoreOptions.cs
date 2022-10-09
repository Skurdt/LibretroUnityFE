/* MIT License

 * Copyright (c) 2022 Skurdt
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UICoreOptions : MonoBehaviour
    {
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private RectTransform _listContent;
        [SerializeField] private UICoreOptionDropdown _dropdownTemplatePrefab;

        private readonly List<GameObject> _instantiatedObjects = new();

        private void OnEnable()
        {
            _inputActions.Disable();
            GenerateOptionsList();
        }

        private void OnDisable()
        {
            _inputActions.Enable();

            for (int i = _instantiatedObjects.Count - 1; i >= 0; --i)
                Destroy(_instantiatedObjects[i]);

            _instantiatedObjects.Clear();
        }

        private void GenerateOptionsList()
        {
            _instantiatedObjects.Clear();

            string coreName = _uiRoot.Libretro.Current.CoreName;
            if (!CoreInstances.Instance.Contains(coreName))
                return;

            (CoreOptions coreOptions, CoreOptions gameOptions) = CoreInstances.Instance[coreName];

            int optionIndex = 0;
            foreach (CoreOption coreOption in coreOptions)
            {
                if (!coreOption.Visible)
                    continue;
                UICoreOptionDropdown optionInstance = Instantiate(_dropdownTemplatePrefab, _listContent);
                optionInstance.Init(coreName, coreOption, gameOptions?[optionIndex]?.CurrentValue);
                _instantiatedObjects.Add(optionInstance.gameObject);
                ++optionIndex;
            }
        }
    }
}
