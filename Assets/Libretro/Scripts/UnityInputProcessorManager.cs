using SK.Libretro.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Libretro
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class UnityInputProcessorManager : MonoBehaviour, IInputProcessor
    {
        private readonly Dictionary<int, UnityInputProcessorComponent> _controls = new Dictionary<int, UnityInputProcessorComponent>();

#pragma warning disable IDE0051 // Remove unused private members, Callbacks for the PlayerInputManager component
        private void OnPlayerJoined(PlayerInput player)
        {
            Log.Info($"Player #{player.playerIndex} joined ({player.currentControlScheme}).");
            _controls.Add(player.playerIndex, player.gameObject.GetComponent<UnityInputProcessorComponent>());
        }

        private void OnPlayerLeft(PlayerInput player)
        {
            Log.Info($"Player #{player.playerIndex} left ({player.currentControlScheme}).");
            _ = _controls.Remove(player.playerIndex);
        }
#pragma warning restore IDE0051 // Remove unused private members

        public bool JoypadButton(int port, int button) => _controls.ContainsKey(port) ? _controls[port].JoypadButtons[button] : false;

        public float MouseDelta(int port, int axis)      => _controls.ContainsKey(port) ? (axis == 0 ? _controls[port].MousePositionDelta.x : -_controls[port].MousePositionDelta.y) : 0f;
        public float MouseWheelDelta(int port, int axis) => _controls.ContainsKey(port) ? (axis == 0 ? _controls[port].MouseWheelDelta.y : _controls[port].MouseWheelDelta.x) : 0f;
        public bool MouseButton(int port, int button)    => _controls.ContainsKey(port) ? _controls[port].MouseButtons[button] : false;
    }
}
