using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        private void RetroInputPollCallback()
        {
        }

        private short RetroInputStateCallback(uint port, RetroDevice device, uint index, uint id)
        {
            short result = 0;

            if (port == 0)
            {
                switch (device)
                {
                    case RetroDevice.RETRO_DEVICE_JOYPAD:
                    {
                        result = ProcessJoypad(id);
                    }
                    break;
                    case RetroDevice.RETRO_DEVICE_MOUSE:
                    {
                        result = ProcessMouse(id);
                    }
                    break;
                    case RetroDevice.RETRO_DEVICE_KEYBOARD:
                    {
                        result = ProcessKeyboard(id);
                    }
                    break;
                    case RetroDevice.RETRO_DEVICE_LIGHTGUN:
                    case RetroDevice.RETRO_DEVICE_ANALOG:
                    case RetroDevice.RETRO_DEVICE_POINTER:
                    case RetroDevice.RETRO_DEVICE_NONE:
                    default:
                        break;
                }
            }

            return result;
        }

        private short ProcessJoypad(uint id)
        {
            bool button = false;

            RetroPad retroPad = (RetroPad)id;
            switch (retroPad)
            {
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_B:
                    button = Input.GetKey(KeyCode.Keypad2);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_Y:
                    button = Input.GetKey(KeyCode.Keypad5);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_SELECT:
                    button = Input.GetKey(KeyCode.RightShift);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_START:
                    button = Input.GetKey(KeyCode.Return);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_UP:
                    button = Input.GetKey(KeyCode.Z);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_DOWN:
                    button = Input.GetKey(KeyCode.S);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_LEFT:
                    button = Input.GetKey(KeyCode.Q);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_RIGHT:
                    button = Input.GetKey(KeyCode.D);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_A:
                    button = Input.GetKey(KeyCode.Keypad1);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_X:
                    button = Input.GetKey(KeyCode.Keypad4);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L:
                    button = Input.GetKey(KeyCode.Keypad6);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R:
                    button = Input.GetKey(KeyCode.Keypad3);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L2:
                    button = Input.GetKey(KeyCode.X);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R2:
                    button = Input.GetKey(KeyCode.C);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L3:
                    button = Input.GetKey(KeyCode.V);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R3:
                    button = Input.GetKey(KeyCode.B);
                    break;
                default:
                    break;
            }

            return BoolToShort(button);
        }

        private short ProcessMouse(uint id)
        {
            short result = 0;

            RetroMouse mouseId = (RetroMouse)id;
            switch (mouseId)
            {
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_X:
                    result = FloatToShort(Input.GetAxis("Mouse X") * 10f);
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_Y:
                    result = FloatToShort(-Input.GetAxis("Mouse Y") * 10f);
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                    result = BoolToShort(Input.GetMouseButton(0));
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                    result = BoolToShort(Input.GetMouseButton(1));
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                    result = BoolToShort(Input.GetMouseButton(2));
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_BUTTON_4:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_BUTTON_5:
                    break;
                default:
                    break;
            }

            return result;
        }

        private short ProcessKeyboard(uint id)
        {
            return BoolToShort(id < (uint)RetroKey.RETROK_OEM_102 ? Input.GetKey((KeyCode)id) : false);
        }

        private static short BoolToShort(bool boolValue)
        {
            return (short)(boolValue ? 1 : 0);
        }

        private static short FloatToShort(float floatValue)
        {
            return (short)Mathf.Clamp(Mathf.Round(floatValue), short.MinValue, short.MaxValue);
        }
    }
}
