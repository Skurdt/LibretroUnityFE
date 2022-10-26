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

using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class UI_ToolbarStateSlotInputField : MonoBehaviour
    {
        //private UI_ToolbarMenu _menu;
        //private TMP_InputField _inputField;
        //private LibretroInstanceVariable _libretro;

        //private void Awake()
        //{
        //    _menu       = transform.parent.GetComponentInParent<UI_ToolbarMenu>(true);
        //    _inputField = GetComponent<TMP_InputField>();
        //    _libretro   = _menu.ToolbarButton.Toolbar.Root.Libretro;
        //}

        //private void OnEnable() => _inputField.onValueChanged.AddListener((string text) =>
        //{
        //    if (int.TryParse(text, out int currentSlot))
        //        _libretro.Current.SetStateSlot(math.clamp(currentSlot, 0, 999999));
        //});

        //private void OnDisable() => _inputField.onValueChanged.RemoveAllListeners();

        //public void DecreaseStateSlot()
        //{
        //    if (int.TryParse(_inputField.text, out int currentSlot))
        //        _inputField.text = math.max(0, --currentSlot).ToString();
        //}

        //public void IncreaseStateSlot()
        //{
        //    if (int.TryParse(_inputField.text, out int currentSlot))
        //        _inputField.text = math.min(++currentSlot, 999999).ToString();
        //}
    }
}
