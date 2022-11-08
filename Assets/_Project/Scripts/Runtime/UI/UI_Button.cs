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
using UnityEngine.UI;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, RequireComponent(typeof(Button))]
    public sealed class UI_Button : MonoBehaviour
    {
        public string Text { get => _text.text; set => _text.SetText(value); }

        private Button _button;
        private TMP_Text _text;
        private Color _textColor;

        public void Construct(bool visible, bool interactable, UnityAction callback)
        {
            _button    = GetComponent<Button>();
            _text      = GetComponentInChildren<TMP_Text>();
            _textColor = _text.color;

            SetVisible(visible);
            SetInteractable(interactable);
            SetCallback(callback);
        }

        public void SetCallback(UnityAction action)
        {
            if (action is null)
                return;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(action);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        public void SetInteractable(bool interactable)
        {
            if (_button)
                _button.interactable = interactable;

            if (_text)
                _text.color = interactable ? _textColor : _text.color * 0.5f;
        }
    }
}
