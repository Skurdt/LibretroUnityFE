///* MIT License

// * Copyright (c) 2020 Skurdt
// *
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files (the "Software"), to deal
// * in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:

// * The above copyright notice and this permission notice shall be included in all
// * copies or substantial portions of the Software.

// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// * SOFTWARE. */

//using UnityEngine;

//namespace SK.Examples.Common
//{
//    [RequireComponent(typeof(Rigidbody))]
//    public class FPSControllerRB : MonoBehaviour
//    {
//        public bool InputEnabled = true;

//        private GameModelSetup _currentGame;

//        public float camRotationSpeed    = 4f;
//        public float camMinimumY         = -88f;
//        public float camMaximumY         = 88f;
//        public float rotationSmoothSpeed = 10f;

//        public float walkSpeed = 3f;
//        public float runSpeed  = 6f;
//        public float maxSpeed  = 10f;
//        public float jumpPower = 10f;

//        public float extraGravity = 8f;

//        public bool grounded;

//        private float bodyRotationX;
//        private float camRotationY;
//        private Vector3 directionIntentX;
//        private Vector3 directionIntentY;
//        private float speed;

//        private Transform _camera;
//        private Rigidbody _rigidbody;

//        private void Awake()
//        {
//            _camera    = GetComponentInChildren<Camera>().transform;
//            _rigidbody = GetComponent<Rigidbody>();
//        }

//        private void Start()
//        {
//            Cursor.lockState = CursorLockMode.Locked;
//            Cursor.visible   = false;
//        }

//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.F))
//            {
//                InputEnabled = !InputEnabled;
//                Debug.Log($"Player input enabled: {InputEnabled}");

//                if (_currentGame != null)
//                {
//                    _currentGame.InputEnabled = !_currentGame.InputEnabled;
//                    Debug.Log($"Game input {(_currentGame.InputEnabled ? "gained" : "lost")}: {_currentGame.Game.Name}");
//                }
//            }

//            ExtraGravity();
//            GroundCheck();

//            if (Cursor.lockState == CursorLockMode.Locked && InputEnabled)
//            {
//                LookRotation();
//                Movement();
//                if (grounded && Input.GetButtonDown("Jump"))
//                {
//                    Jump();
//                }
//            }
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            _currentGame = other.GetComponent<GameModelSetup>();
//        }

//        private void OnTriggerExit(Collider other)
//        {
//            if (_currentGame != null)
//            {
//                _currentGame.InputEnabled = false;
//            }
//            _currentGame = null;
//        }

//        private void LookRotation()
//        {
//            bodyRotationX += Input.GetAxis("Mouse X") * camRotationSpeed;
//            camRotationY  += Input.GetAxis("Mouse Y") * camRotationSpeed;

//            camRotationY = Mathf.Clamp(camRotationY, camMinimumY, camMaximumY);

//            Quaternion camTargetRotation  = Quaternion.Euler(-camRotationY, 0f, 0f);
//            Quaternion bodyTargetRotation = Quaternion.Euler(0f, bodyRotationX, 0f);

//            transform.rotation = Quaternion.Lerp(transform.rotation, bodyTargetRotation, rotationSmoothSpeed * Time.deltaTime);

//            _camera.localRotation = Quaternion.Lerp(_camera.localRotation, camTargetRotation, rotationSmoothSpeed * Time.deltaTime);
//        }

//        private void Movement()
//        {
//            directionIntentX   = _camera.right;
//            directionIntentX.y = 0f;
//            directionIntentX.Normalize();

//            directionIntentY   = _camera.forward;
//            directionIntentY.y = 0f;
//            directionIntentY.Normalize();

//            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

//            _rigidbody.velocity =
//                directionIntentY * Input.GetAxis("Vertical") * speed
//                + directionIntentX * Input.GetAxis("Horizontal") * speed
//                + Vector3.up * _rigidbody.velocity.y;
//            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
//        }

//        private void ExtraGravity()
//        {
//            _rigidbody.AddForce(Vector3.down * extraGravity);
//        }

//        private void GroundCheck()
//        {
//            grounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit _, 1.25f);
//        }

//        private void Jump()
//        {
//            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
//        }
//    }
//}
