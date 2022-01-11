﻿/* MIT License

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
using System.Collections.Generic;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UIInputDevices : MonoBehaviour
    {
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private UIInputDevice _uiInputDevicePrefab;

        private readonly List<UIInputDevice> _uiInputDevices = new();

        private void OnEnable()
        {
            _uiInputDevices.Clear();
            int portIndex = 0;
            foreach (Controllers controllers in _uiRoot.Libretro.Current.ControllersMap)
            {
                UIInputDevice device = Instantiate(_uiInputDevicePrefab, transform);
                device.Init(portIndex, controllers);
                _uiInputDevices.Add(device);
                ++portIndex;
            }
        }

        private void OnDisable()
        {
            for (int i = _uiInputDevices.Count - 1; i >= 0; --i)
                Destroy(_uiInputDevices[i].gameObject);
            _uiInputDevices.Clear();
        }
    }
}
