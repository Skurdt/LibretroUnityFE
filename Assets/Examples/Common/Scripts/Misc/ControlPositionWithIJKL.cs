using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Examples.Common
{
    public class ControlPositionWithIJKL : MonoBehaviour
    {
        private void Update()
        {
            Vector3 velocity = Vector3.zero;

            if (Keyboard.current.iKey.isPressed)
            {
                velocity += new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.kKey.isPressed)
            {
                velocity -= new Vector3(0f, 0f, 1f);
            }

            if (Keyboard.current.jKey.isPressed)
            {
                velocity -= new Vector3(1.0f, 0f, 0f);
            }

            if (Keyboard.current.lKey.isPressed)
            {
                velocity += new Vector3(1.0f, 0f, 0f);
            }

            transform.Translate(velocity.normalized * Time.deltaTime);
        }
    }
}
