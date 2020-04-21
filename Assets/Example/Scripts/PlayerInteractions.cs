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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Common
{
    [RequireComponent(typeof(PlayerControls))]
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private float _raycastMaxDistance = 1.2f;
        [SerializeField] private LayerMask _raycastLayerMask;

        private PlayerControls _playerController;
        private Camera _playerCamera;

        private readonly Stack<GameModelSetup> _gameStack = new Stack<GameModelSetup>(1);
        private GameModelSetup _currentGame;

        private void Awake()
        {
            _playerController  = GetComponent<PlayerControls>();
            _playerCamera      = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            _ = Utils.HideMouseCursor();
        }

        private void Update()
        {
            GameModelSetup gameFromRay = GetGameFromRaycast();
            if (gameFromRay != null)
            {
                if (_currentGame != gameFromRay)
                {
                    if (_gameStack.Count > 0)
                    {
                        _gameStack.Pop().DeactivateInput();
                    }

                    _currentGame = gameFromRay;
                    _gameStack.Push(_currentGame);
                }
            }
            else
            {
                if (_currentGame != null)
                {
                    _currentGame.ActivateInput();
                    _currentGame = null;
                }
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (_currentGame != null)
                {
                    if (_playerController.InputEnabled)
                    {
                        _playerController.DeactivateInput();
                        _currentGame.ActivateInput();
                    }
                    else
                    {
                        _playerController.ActivateInput();
                        _currentGame.DeactivateInput();
                    }
                }
            }

            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                _ = Utils.ToggleMouseCursor();
                if (_playerController.InputEnabled)
                {
                    _playerController.DeactivateInput();
                    if (_currentGame != null)
                    {
                        _currentGame.DeactivateInput();
                    }
                }
                else
                {
                    _playerController.ActivateInput();
                    if (_currentGame != null)
                    {
                        _currentGame.ActivateInput();
                    }
                }
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Utils.ExitApp();
            }
        }

        private GameModelSetup GetGameFromRaycast()
        {
            Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _raycastMaxDistance, _raycastLayerMask))
            {
                return hitInfo.transform.GetComponent<GameModelSetup>();
            }
            return null;
        }
    }
}
