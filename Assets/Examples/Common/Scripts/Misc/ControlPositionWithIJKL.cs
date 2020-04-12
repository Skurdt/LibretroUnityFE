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

namespace SK.Examples.Common
{
    public class ControlPositionWithIJKL : MonoBehaviour
    {
        private void Update()
        {
            Vector3 velocity = Vector3.zero;

            if (Keyboard.current.iKey.isPressed)
            {
                velocity += new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.kKey.isPressed)
            {
                velocity -= new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.jKey.isPressed)
            {
                velocity -= new Vector3(1.0f, 0f, 0f);
            }

            if (Keyboard.current.lKey.isPressed)
            {
                velocity += new Vector3(1.0f, 0f, 0f);
            }

            transform.Translate(velocity.normalized * Time.deltaTime);
        }
    }
}
