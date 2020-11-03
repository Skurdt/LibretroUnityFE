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

using SK.UnityUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Player
{
    internal sealed class GameFocusState : State
    {
        private GameModelSetupInteractive _currentGame;

        public GameFocusState(StateController stateController, Controls controls, Interactions interactions)
        : base(stateController, controls, interactions)
        {
        }

        public override void OnEnter()
        {
            _controls.InputEnabled    = false;
            _currentGame              = _interactions.GetCurrentGame();
            _currentGame.InputEnabled = true;
        }

        public override void OnUpdate(float dt)
        {
            if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame)
            {
                CursorUtils.ToggleMouseCursor();
                _currentGame.InputEnabled = !_currentGame.InputEnabled;
            }

            if (!Cursor.visible)
            {
                if ((Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) || (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame))
                    _stateController.TransitionTo<NormalState>();

                if (Keyboard.current != null)
                {
                    if (Keyboard.current.pKey.wasPressedThisFrame)
                    {
                        if (_currentGame.Running)
                            _currentGame.Pause();
                        else
                            _currentGame.Resume();
                    }

                    if (Keyboard.current.f5Key.wasPressedThisFrame && _currentGame.SaveState(0))
                        Debug.Log("State saved to slot 0");
                    if (Keyboard.current.f6Key.wasPressedThisFrame && _currentGame.SaveState(1))
                        Debug.Log("State saved to slot 1");
                    if (Keyboard.current.f7Key.wasPressedThisFrame && _currentGame.SaveState(2))
                        Debug.Log("State saved to slot 2");
                    if (Keyboard.current.f8Key.wasPressedThisFrame && _currentGame.SaveState(3))
                        Debug.Log("State saved to slot 3");

                    if (Keyboard.current.f9Key.wasPressedThisFrame && _currentGame.LoadState(0))
                        Debug.Log("State loaded from slot 0");
                    if (Keyboard.current.f10Key.wasPressedThisFrame && _currentGame.LoadState(1))
                        Debug.Log("State loaded from slot 1");
                    if (Keyboard.current.f11Key.wasPressedThisFrame && _currentGame.LoadState(2))
                        Debug.Log("State loaded from slot 2");
                    if (Keyboard.current.f12Key.wasPressedThisFrame && _currentGame.LoadState(3))
                        Debug.Log("State loaded from slot 3");

                    _currentGame.Rewind(Keyboard.current.backspaceKey.isPressed);
                }
            }
        }

        public override void OnExit()
        {
            _currentGame.InputEnabled = false;
            _currentGame              = null;
            _controls.InputEnabled    = true;
        }
    }
}
