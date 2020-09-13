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

namespace SK.Examples.Player
{
    [RequireComponent(typeof(Controls))]
    public class Interactions : MonoBehaviour
    {
        [SerializeField] private float _raycastMaxDistance = 1.2f;
#pragma warning disable 0649
        [SerializeField] private LayerMask _raycastLayerMask;
#pragma warning restore 0649
        private Controls _controls;
        private Camera _camera;

        private StateController _stateController;

        private GameModelSetup _currentGame;

        private void Awake()
        {
            _controls        = GetComponent<Controls>();
            _camera          = GetComponentInChildren<Camera>();
            _stateController = new StateController(_controls, this);
        }

        private void Start()
        {
            Utils.HideMouseCursor();
            _stateController.TransitionTo<NormalState>();
        }

        private void Update()
        {
            _stateController.Update(Time.deltaTime);

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Utils.ExitApp();
            }
        }

        private void OnGUI()
        {
            if (_currentGame == null
             || _currentGame.Wrapper == null
             || _stateController.CurrentState == null
             || _stateController.CurrentState as GameFocusState == null)
            {
                return;
            }

            float labelWidth = 100f;
            float fieldWidth = 100f;
            float height = 20f;

            // Show options
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Crop Overscan:", GUILayout.Width(labelWidth), GUILayout.Height(height));
                _currentGame.Wrapper.OptionCropOverscan = GUILayout.Toggle(_currentGame.Wrapper.OptionCropOverscan, string.Empty, GUILayout.Width(fieldWidth), GUILayout.Height(height));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Volume:", GUILayout.Width(labelWidth), GUILayout.Height(height));
                _currentGame.AudioMaxVolume = GUILayout.HorizontalSlider(_currentGame.AudioMaxVolume, 0f, 1f, GUILayout.Width(fieldWidth), GUILayout.Height(height));
            }
            GUILayout.EndHorizontal();
        }

        public GameModelSetup GetCurrentGame()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _raycastMaxDistance, _raycastLayerMask))
            {
                GameModelSetup gameFromRay = hitInfo.transform.GetComponent<GameModelSetup>();
                if (_currentGame != gameFromRay)
                {
                    _currentGame = gameFromRay;
                }
            }
            else
            {
                _currentGame = null;
            }

            return _currentGame;
        }
    }
}
