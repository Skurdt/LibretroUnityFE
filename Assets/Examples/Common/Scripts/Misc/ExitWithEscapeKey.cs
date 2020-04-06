using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Common
{
    public class ExitWithEscapeKey : MonoBehaviour
    {
        private void Update()
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
