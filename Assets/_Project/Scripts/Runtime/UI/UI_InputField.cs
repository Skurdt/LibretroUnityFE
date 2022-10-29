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

using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, RequireComponent(typeof(TMP_InputField))]
    public sealed class UI_InputField : MonoBehaviour
    {
        public string Text => _inputField.text;

        private TMP_InputField _inputField;

        public void Construct(bool visible, UnityAction<string> callback)
        {
            _inputField = GetComponentInChildren<TMP_InputField>();

            SetVisible(visible);
            SetCallback(callback);
        }

        public void SetCallback(UnityAction<string> action)
        {
            if (action is null)
                return;

            _inputField.onValueChanged.RemoveAllListeners();
            _inputField.onValueChanged.AddListener(action);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
