using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Common
{
    public class ToggleCursorWithMiddleMouseButton : MonoBehaviour
    {
        private void Update()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible   = true;
                }
                else if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState   = CursorLockMode.Locked;
                    Cursor.visible     = false;
                }
            }
        }
    }
}
