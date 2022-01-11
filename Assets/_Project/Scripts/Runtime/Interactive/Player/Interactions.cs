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

using SK.Libretro.Unity;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, RequireComponent(typeof(Controls))]
    public sealed class Interactions : MonoBehaviour
    {
        public LibretroInstanceVariable Libretro;

        [SerializeField] private float _raycastMaxDistance = 1.2f;

        private Controls _controls;
        private Camera _camera;
        private StateController _stateController;

        private void Awake()
        {
            _controls        = GetComponent<Controls>();
            _camera          = GetComponentInChildren<Camera>();
            _stateController = new StateController(_controls, this);
        }

        private void Start() => _stateController.TransitionTo<NormalState>();

        private void Update() => _stateController.Update(Time.deltaTime);

        private void FixedUpdate() => _stateController.FixedUpdate(Time.fixedDeltaTime);

        public void UpdateCurrentTargetReference()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            Libretro.Current = Physics.Raycast(ray, out RaycastHit hitInfo, _raycastMaxDistance)
                                     ? hitInfo.transform.GetComponentInChildren<LibretroInstance>()
                                     : null;
        }
    }
}
