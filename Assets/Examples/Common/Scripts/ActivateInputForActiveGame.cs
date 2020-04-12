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

using System.Linq;
using UnityEngine;

namespace SK.Examples.Common
{
    [RequireComponent(typeof(Camera))]
    public class ActivateInputForActiveGame : MonoBehaviour
    {
        [SerializeField] private float _gameInputMaxDistance = 2.4f;

        private Camera _mainCamera;
        private GameModelSetup _currentGame;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _gameInputMaxDistance))
            {
                if (hit.transform.gameObject.TryGetComponent(out GameModelSetup hitModelSetup))
                {
                    if (_currentGame != hitModelSetup)
                    {
                        _currentGame = hitModelSetup;
                        _currentGame.ActivateInput();

                        GameModelSetup[] modelSetups = FindObjectsOfType<GameModelSetup>().Where(x => x != _currentGame).ToArray();
                        for (int i = 0; i < modelSetups.Length; ++i)
                        {
                            modelSetups[i].DeactivateInput();
                        }
                    }
                }
            }
            else
            {
                if (_currentGame != null)
                {
                    _currentGame.DeactivateInput();
                    _currentGame = null;
                }
            }
        }
    }
}
