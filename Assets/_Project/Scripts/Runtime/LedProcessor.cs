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
using System.Collections.Concurrent;
using UnityEngine;

namespace SK.Libretro.Examples
{
    public sealed class LedProcessor : LedProcessorBase
    {
        [SerializeField] private LedObject _ledObjectPrefab;

        private readonly ConcurrentDictionary<int, LedObject> _ledDictionary = new();

        protected override void OnSetState(int led, int state)
        {
            if (!_ledObjectPrefab)
                return;

            if (!_ledDictionary.TryGetValue(led, out LedObject ledObject))
            {
                ledObject = Instantiate(_ledObjectPrefab, transform);
                ledObject.name = $"Led_{led:00}";
                _ = _ledDictionary.TryAdd(led, ledObject);
            }

            ledObject.SetState(state != 0);
        }
    }
}
