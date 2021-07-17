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

using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    [DisallowMultipleComponent]
    public sealed class UICoreOptions : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private RectTransform _listContent;
        [SerializeField] private UICoreOptionToggle _toggleTemplatePrefab;
        [SerializeField] private UICoreOptionDropdown _dropdownTemplatePrefab;

        private void Start() => Hide();

        public void Show()
        {
            _inputActions.Disable();

            //foreach (string item in LibretroBridge.CurrentInstance.CoreOptions.Options)
            //{
            //    UICoreOptionToggle toggle = Instantiate(_toggleTemplatePrefab, _content);
            //    UICoreOptionDropdown dropDown = Instantiate(_dropdownTemplatePrefab, _content);
            //}
        }

        public void Hide() => _inputActions.Enable();
    }
}
