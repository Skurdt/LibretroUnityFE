using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private void RetroInputPollCallback()
        {
        }

        private short RetroInputStateCallback(uint port, retro_device device, uint index, uint id)
        {
            short result = 0;

            if (port == 0)
            {
                switch (device)
                {
                    case retro_device.RETRO_DEVICE_JOYPAD:
                    {
                        result = ProcessJoypad((retro_device_id_joypad)id);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_MOUSE:
                    {
                        result = ProcessMouse((retro_device_id_mouse)id);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_KEYBOARD:
                    {
                        result = ProcessKeyboard((retro_key)id);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_LIGHTGUN:
                    case retro_device.RETRO_DEVICE_ANALOG:
                    case retro_device.RETRO_DEVICE_POINTER:
                    case retro_device.RETRO_DEVICE_NONE:
                    default:
                        break;
                }
            }

            return result;
        }

        private short ProcessJoypad(retro_device_id_joypad id)
        {
            short result = 0;

            switch (id)
            {
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_UP:
                    result = BoolToShort(Input.GetKey(KeyCode.Z));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_DOWN:
                    result = BoolToShort(Input.GetKey(KeyCode.S));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_LEFT:
                    result = BoolToShort(Input.GetKey(KeyCode.Q));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_RIGHT:
                    result = BoolToShort(Input.GetKey(KeyCode.D));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_START:
                    result = BoolToShort(Input.GetKey(KeyCode.Return));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_SELECT:
                    result = BoolToShort(Input.GetKey(KeyCode.RightShift));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_A:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad1));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_B:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad2));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_X:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad4));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_Y:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad5));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad6));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R:
                    result = BoolToShort(Input.GetKey(KeyCode.Keypad3));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L2:
                    result = BoolToShort(Input.GetKey(KeyCode.X));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R2:
                    result = BoolToShort(Input.GetKey(KeyCode.C));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_L3:
                    result = BoolToShort(Input.GetKey(KeyCode.V));
                    break;
                case retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_R3:
                    result = BoolToShort(Input.GetKey(KeyCode.B));
                    break;
                default:
                    break;
            }

            return result;
        }

        private short ProcessMouse(retro_device_id_mouse id)
        {
            short result = 0;

            switch (id)
            {
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_X:
                    result = FloatToShort(Input.GetAxisRaw("Mouse X") * 10f);
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_Y:
                    result = FloatToShort(-Input.GetAxisRaw("Mouse Y") * 10f);
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                    result = BoolToShort(Input.GetMouseButton(0));
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                    result = BoolToShort(Input.GetMouseButton(1));
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                    result = FloatToShort(Input.GetAxisRaw("Mouse ScrollWheel") * 10f);
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                    result = FloatToShort(-Input.GetAxisRaw("Mouse ScrollWheel") * 10f);
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                    result = BoolToShort(Input.GetMouseButton(2));
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_4:
                    break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_5:
                    break;
                default:
                    break;
            }

            return result;
        }

        private short ProcessKeyboard(retro_key id)
        {
            return BoolToShort(id < retro_key.RETROK_OEM_102 ? Input.GetKey((KeyCode)id) : false);
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
