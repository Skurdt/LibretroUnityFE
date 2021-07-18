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

using SK.Utilities.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples
{
    public sealed class GameFocusState : State
    {
        public GameFocusState(StateController stateController, Controls controls, Interactions interactions)
        : base(stateController, controls, interactions)
        {
        }

        public override void OnEnter()
        {
            _controls.InputEnabled                 = false;
            //_interactions.CurrentGame.InputEnabled = true;
        }

        public override void OnUpdate(float dt)
        {
            if (!(Mouse.current is null) && Mouse.current.middleButton.wasPressedThisFrame)
            {
                CursorUtils.ToggleMouseCursor();
                //_interactions.CurrentGame.InputEnabled = !_interactions.CurrentGame.InputEnabled;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (!(Keyboard.current is null))
                {
                    if (Keyboard.current.eKey.wasPressedThisFrame || (!(Gamepad.current is null) && Gamepad.current.rightStickButton.wasPressedThisFrame))
                        _stateController.TransitionTo<NormalState>();

                    //if (Keyboard.current.pKey.wasPressedThisFrame)
                    //{
                    //    if (_interactions.CurrentGame.Paused)
                    //        _interactions.CurrentGame.Resume();
                    //    else
                    //        _interactions.CurrentGame.Pause();
                    //}

                    //if (Keyboard.current.f5Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.SaveState(0);
                    //    Debug.Log("State saved to slot 0");
                    //}
                    //if (Keyboard.current.f6Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.SaveState(1);
                    //    Debug.Log("State saved to slot 1");
                    //}
                    //if (Keyboard.current.f7Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.SaveState(2);
                    //    Debug.Log("State saved to slot 2");
                    //}
                    //if (Keyboard.current.f8Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.SaveState(3);
                    //    Debug.Log("State saved to slot 3");
                    //}

                    //if (Keyboard.current.f9Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.LoadState(0);
                    //    Debug.Log("State saved to slot 0");
                    //}
                    //if (Keyboard.current.f10Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.LoadState(1);
                    //    Debug.Log("State saved to slot 1");
                    //}
                    //if (Keyboard.current.f11Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.LoadState(2);
                    //    Debug.Log("State saved to slot 2");
                    //}
                    //if (Keyboard.current.f12Key.wasPressedThisFrame)
                    //{
                    //    _interactions.CurrentGame.LoadState(3);
                    //    Debug.Log("State saved to slot 3");
                    //}

                    //_interactions.CurrentGame.Rewind(Keyboard.current.backspaceKey.isPressed);
                }
            }
        }

        public override void OnExit()
        {
            //_interactions.CurrentGame.InputEnabled = false;
            _controls.InputEnabled = true;
        }
    }
}
