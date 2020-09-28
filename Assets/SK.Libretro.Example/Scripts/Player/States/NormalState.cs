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

using UnityEngine.InputSystem;

namespace SK.Examples.Player
{
    public class NormalState : State
    {
        public NormalState(StateController stateController, Controls controls, Interactions interactions)
        : base(stateController, controls, interactions)
        {
        }

        public override void OnEnter() => _controls.InputEnabled = true;

        public override void OnUpdate(float dt)
        {
            if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame)
            {
                Utils.ToggleMouseCursor();
                _controls.InputEnabled = !_controls.InputEnabled;
            }

            if (_interactions.GetCurrentGame() != null)
            {
                if ((Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) || (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame))
                {
                    _stateController.TransitionTo<GameFocusState>();
                }
            }
        }

        public override void OnExit() => _controls.InputEnabled = false;
    }
}
