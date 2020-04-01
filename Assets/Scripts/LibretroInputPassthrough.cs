using UnityEngine;
using UnityEngine.InputSystem;

namespace SK
{
    [RequireComponent(typeof(PlayerInput))]
    public class LibretroInputPassthrough : MonoBehaviour
    {
        private GameModelSetup _gms;
        private int _playerIndex;

        private void Awake()
        {
            _gms = FindObjectOfType<GameModelSetup>();
            transform.parent = _gms.transform;
            _playerIndex = GetComponent<PlayerInput>().user.index;
        }

#pragma warning disable IDE0051 // Remove unused private members (These are called by the PlayerInput Component)
        private void OnUp(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_UP] = val.isPressed;
        }

        private void OnDown(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_DOWN] = val.isPressed;
        }

        private void OnLeft(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_LEFT] = val.isPressed;
        }

        private void OnRight(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_RIGHT] = val.isPressed;
        }

        private void OnStart(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_START] = val.isPressed;
        }

        private void OnSelect(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_SELECT] = val.isPressed;
        }

        private void OnA(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_A] = val.isPressed;
        }

        private void OnB(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_B] = val.isPressed;
        }

        private void OnX(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_X] = val.isPressed;
        }

        private void OnY(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_Y] = val.isPressed;
        }
        private void OnL(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L] = val.isPressed;
        }

        private void OnR(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R] = val.isPressed;
        }

        private void OnL2(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L2] = val.isPressed;
        }

        private void OnR2(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R2] = val.isPressed;
        }

        private void OnL3(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L3] = val.isPressed;
        }

        private void OnR3(InputValue val)
        {
            if (_gms == null || _gms.Wrapper == null)
            {
                return;
            }
            _gms.Wrapper.RetroPadButtons[_playerIndex].Buttons[(int)Libretro.Wrapper.retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R3] = val.isPressed;
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
