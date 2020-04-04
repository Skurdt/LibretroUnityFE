using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    public class Main : MonoBehaviour
    {
        private Camera _mainCamera;
        private GameModelSetup _currentGame;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            GetActiveGame();

            float velocityX = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocityX -= 0.5f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocityX += 0.5f;
            }

            transform.Translate(velocityX * Time.deltaTime, 0f, 0f);

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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(0);
#endif
            }
        }

        private void GetActiveGame()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                if (hit.transform.gameObject.TryGetComponent(out GameModelSetup modelSetup))
                {
                    if (_currentGame != modelSetup)
                    {
                        _currentGame = modelSetup;

                        GameModelSetup[] wrappers = FindObjectsOfType<GameModelSetup>();
                        foreach (GameModelSetup item in wrappers)
                        {
                            if (item != _currentGame)
                            {
                                item.DeactivateInput();
                            }
                        }

                        _currentGame.ActivateInput();
                    }
                }
            }
        }
    }
}
