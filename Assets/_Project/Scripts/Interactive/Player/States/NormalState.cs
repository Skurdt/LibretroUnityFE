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
    public sealed class NormalState : State
    {
        private bool _haveKeyboard;
        private bool _haveMouse;
        private bool _haveGamepad;

        public NormalState(StateController stateController, Controls controls, Interactions interactions)
        : base(stateController, controls, interactions)
        {
        }

        public override void OnEnter()
        {
            _controls.InputEnabled = true;
            _haveKeyboard = Keyboard.current != null;
            _haveMouse    = Mouse.current != null;
            _haveGamepad  = Gamepad.current != null;
        }

        public override void OnUpdate(float dt)
        {
            if (_haveMouse && Mouse.current.middleButton.wasPressedThisFrame)
            {
                CursorUtils.ToggleMouseCursor();
                _controls.InputEnabled = Cursor.lockState != CursorLockMode.None;
            }

            _interactions.UpdateCurrentTargetReference();

            bool keyboardInteract = _haveKeyboard && Keyboard.current.eKey.wasPressedThisFrame;
            bool gamepadInteract  = _haveGamepad && Gamepad.current.buttonSouth.wasPressedThisFrame;
            if (keyboardInteract || gamepadInteract)
            {
                _interactions.Libretro.Current.StartContent();
                _stateController.TransitionTo<GameFocusState>();
            }
        }

        public override void OnExit() => _controls.InputEnabled = false;
    }
}
