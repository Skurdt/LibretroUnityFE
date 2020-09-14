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
    [RequireComponent(typeof(CharacterController))]
    public class Controls : MonoBehaviour
    {
        public bool InputEnabled;

        public float WalkSpeed = 2f;
        public float RunSpeed  = 4f;
        public float JumpForce = 5f;

        public float LookSensitivityHorizontal = 3f;
        public float LookSensitivityVertical   = 2f;

        private const float MIN_ROTATION_ANGLE = -89f;
        private const float MAX_ROTATION_ANGLE = 89f;
        private const float EXTRA_GRAVITY      = 20f;

        private CharacterController _characterController = null;
        private Camera _camera                           = null;
        private float _lookHorizontal                    = 0f;
        private float _lookVertical                      = 0f;
        private Vector3 _moveVelocity                    = Vector3.zero;

        private float _movementInputX;
        private float _movementInputY;
        private float _lookInputH;
        private float _lookInputV;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _camera              = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            if (InputEnabled)
            {
                GatherInputValues();

                if (_characterController.isGrounded)
                {
                    _moveVelocity = new Vector3(_movementInputX, -0.1f, _movementInputY);
                    _moveVelocity.Normalize();

                    float speed = (Keyboard.current.leftShiftKey.isPressed) || (Gamepad.current!=null && Gamepad.current.xButton.isPressed) ? RunSpeed : WalkSpeed;
                    _moveVelocity = transform.TransformDirection(_moveVelocity) * speed;

                    if (Keyboard.current.spaceKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame))
                    {
                        _moveVelocity.y = JumpForce;
                    }
                }

                _lookHorizontal = LookSensitivityHorizontal * _lookInputH;
                _lookVertical  += LookSensitivityVertical   * _lookInputV;
                _lookVertical = Mathf.Clamp(_lookVertical, MIN_ROTATION_ANGLE, MAX_ROTATION_ANGLE);
                _camera.transform.localEulerAngles = new Vector3(-_lookVertical, 0f, 0f);
                transform.Rotate(new Vector3(0f, _lookHorizontal, 0f));
            }

            _moveVelocity.y -= EXTRA_GRAVITY * Time.deltaTime;
            _ = _characterController.Move(_moveVelocity * Time.deltaTime);
        }

        private void GatherInputValues()
        {
            _movementInputX = Input.GetAxisRaw("Horizontal");
            _movementInputY = Input.GetAxisRaw("Vertical");
            _lookInputH     = Input.GetAxis("Mouse X");
            _lookInputV     = Input.GetAxis("Mouse Y");
        }
    }
}
