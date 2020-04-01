using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    [RequireComponent(typeof(PlayerInput))]
    public class LibretroInputPassthrough : MonoBehaviour
    {
        private GameModelSetup _gameModelSetup;
        private PlayerInput _playerInputComponent;
        private int _playerIndex;

        private void Awake()
        {
            _gameModelSetup       = FindObjectOfType<GameModelSetup>();
            _playerInputComponent = GetComponent<PlayerInput>();
            _playerIndex          = _playerInputComponent.user.index;
        }

        private void OnEnable()
        {
            _playerInputComponent.ActivateInput();
        }

        private void OnDisable()
        {
            _playerInputComponent.DeactivateInput();
        }

#pragma warning disable IDE0051 // Remove unused private members (These are called by the PlayerInput Component)
        private void OnUp(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_UP] = val.isPressed;
        }

        private void OnDown(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_DOWN] = val.isPressed;
        }

        private void OnLeft(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_LEFT] = val.isPressed;
        }

        private void OnRight(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_RIGHT] = val.isPressed;
        }

        private void OnStart(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_START] = val.isPressed;
        }

        private void OnSelect(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_SELECT] = val.isPressed;
        }

        private void OnA(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_A] = val.isPressed;
        }

        private void OnB(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_B] = val.isPressed;
        }

        private void OnX(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_X] = val.isPressed;
        }

        private void OnY(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_Y] = val.isPressed;
        }
        private void OnL(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L] = val.isPressed;
        }

        private void OnR(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R] = val.isPressed;
        }

        private void OnL2(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L2] = val.isPressed;
        }

        private void OnR2(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R2] = val.isPressed;
        }

        private void OnL3(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L3] = val.isPressed;
        }

        private void OnR3(InputValue val)
        {
            if (_gameModelSetup == null || _gameModelSetup.Wrapper == null)
            {
                return;
            }
            _gameModelSetup.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R3] = val.isPressed;
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
