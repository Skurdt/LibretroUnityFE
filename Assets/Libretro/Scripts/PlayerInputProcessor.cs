using SK.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using static SK.Libretro.Wrapper;
using static SK.Libretro.Wrapper.retro_device_id_joypad;

namespace SK
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        private const int NUM_MOUSE_BUTTONS = 5;

        public bool[] JoypadButtons { get; private set; } = new bool[System.Enum.GetNames(typeof(retro_device_id_joypad)).Length];

        public Vector2 MousePositionDelta { get; private set; } = Vector2.zero;
        public Vector2 MouseWheelDelta    { get; private set; } = Vector2.zero;
        public bool[] MouseButtons        { get; private set; } = new bool[NUM_MOUSE_BUTTONS];

#pragma warning disable IDE0051 // Remove unused private members, Callbacks for the PlayerInput component
        private void OnDeviceLost(PlayerInput player)
        {
            Log.Info($"Player #{player.playerIndex} device lost ({player.devices.Count}).");
        }

        private void OnDeviceRegained(PlayerInput player)
        {
            Log.Info($"Player #{player.playerIndex} device regained ({player.devices.Count}).");
        }

        private void OnControlsChanged(PlayerInput player)
        {
            Log.Info($"Player #{player.playerIndex} controls changed ({player.devices.Count}).");
        }

        private void OnJoypadDirections(InputValue value)
        {
            Vector2 vec = value.Get<Vector2>();
            SetJoypadDirection(RETRO_DEVICE_ID_JOYPAD_UP, vec.y > 0.5f);
            SetJoypadDirection(RETRO_DEVICE_ID_JOYPAD_DOWN, vec.y < -0.5f);
            SetJoypadDirection(RETRO_DEVICE_ID_JOYPAD_LEFT, vec.x < -0.5f);
            SetJoypadDirection(RETRO_DEVICE_ID_JOYPAD_RIGHT, vec.x > 0.5f);
        }

        private void OnJoypadStartButton(InputValue value)  => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_START, value);
        private void OnJoypadSelectButton(InputValue value) => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_SELECT, value);
        private void OnJoypadAButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_A, value);
        private void OnJoypadBButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_B, value);
        private void OnJoypadXButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_X, value);
        private void OnJoypadYButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_Y, value);
        private void OnJoypadLButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_L, value);
        private void OnJoypadRButton(InputValue value)      => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_R, value);
        private void OnJoypadL2Button(InputValue value)     => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_L2, value);
        private void OnJoypadR2Button(InputValue value)     => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_R2, value);
        private void OnJoypadL3Button(InputValue value)     => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_L3, value);
        private void OnJoypadR3Button(InputValue value)     => SetJoypadButtonState(RETRO_DEVICE_ID_JOYPAD_R3, value);

        private void OnMousePositionDelta(InputValue value) => SetMousePositionDelta(value);
        private void OnMouseWheelDelta(InputValue value)    => SetMouseWheelDelta(value);
        private void OnMouseLeftButton(InputValue value)    => SetMouseButtonState(0, value);
        private void OnMouseRightButton(InputValue value)   => SetMouseButtonState(1, value);
        private void OnMouseMiddleButton(InputValue value)  => SetMouseButtonState(2, value);
        private void OnMouseForwardButton(InputValue value) => SetMouseButtonState(3, value);
        private void OnMouseBackButton(InputValue value)    => SetMouseButtonState(4, value);
#pragma warning restore IDE0051 // Remove unused private members

        private void SetJoypadDirection(retro_device_id_joypad direction, bool value) => JoypadButtons[(int)direction] = value;
        private void SetJoypadButtonState(retro_device_id_joypad button, InputValue value) => JoypadButtons[(int)button] = value.isPressed;

        private void SetMousePositionDelta(InputValue value)           => MousePositionDelta   = value.Get<Vector2>();
        private void SetMouseWheelDelta(InputValue value)              => MouseWheelDelta      = value.Get<Vector2>();
        private void SetMouseButtonState(int button, InputValue value) => MouseButtons[button] = value.isPressed;
    }
}
