using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    public class Main : MonoBehaviour
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
            GetActiveGame();
            UpdateCursorState();
            UpdateCamera();
            CheckForQuit();
        }

        private void GetActiveGame()
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

                        GameModelSetup[] modelSetups = FindObjectsOfType<GameModelSetup>();
                        for (int i = 0; i < modelSetups.Length; ++i)
                        {
                            if (modelSetups[i] != _currentGame)
                            {
                                modelSetups[i].DeactivateInput();
                            }
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

        private static void UpdateCursorState()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                Cursor.visible = !Cursor.visible;
            }
        }

        private void UpdateCamera()
        {
            Vector3 velocity = Vector3.zero;

            if (Keyboard.current.upArrowKey.isPressed)
            {
                velocity += new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.downArrowKey.isPressed)
            {
                velocity -= new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                velocity -= new Vector3(1.0f, 0f, 0f);
            }

            if (Keyboard.current.rightArrowKey.isPressed)
            {
                velocity += new Vector3(1.0f, 0f, 0f);
            }

            transform.Translate(velocity.normalized * Time.deltaTime);
        }

        private static void CheckForQuit()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(0);
#endif
            }
        }
    }
}
