using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public void RetroInputPollCallback()
        {
        }

        public short RetroInputStateCallback(uint port, retro_device device, uint _/*index*/, uint id)
        {
            short result = 0;

            if (InputProcessor != null && port < 2)
            {
                switch (device)
                {
                    case retro_device.RETRO_DEVICE_JOYPAD:
                    {
                        if (!Core.HasInputDescriptors)
                        {
                            result = ProcessJoypadDeviceState((int)port, (int)id);
                        }
                    }
                    break;
                    case retro_device.RETRO_DEVICE_MOUSE:
                    {
                        result = ProcessMouseDeviceState((int)port, (retro_device_id_mouse)id);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_KEYBOARD:
                    {
                        result = BoolToShort(id < (uint)retro_key.RETROK_OEM_102 ? Input.GetKey((KeyCode)id) : false);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_LIGHTGUN:
                        break;
                    case retro_device.RETRO_DEVICE_ANALOG:
                        break;
                    case retro_device.RETRO_DEVICE_POINTER:
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private short ProcessJoypadDeviceState(int port, int button)
        {
            return BoolToShort(InputProcessor.JoypadButton(port, button));
        }

        private short ProcessMouseDeviceState(int port, retro_device_id_mouse command)
        {
            short result = 0;

            switch (command)
            {
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_X:
                {
                    result = FloatToShort(InputProcessor.MouseDelta(port, 0));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_Y:
                {
                    result = FloatToShort(InputProcessor.MouseDelta(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 0));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                {
                    result = FloatToShort(InputProcessor.MouseWheelDelta(port, 0));
                    if (result != 0f)
                    {
                        Debug.Log(result);
                    }
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 2));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                {
                    result = FloatToShort(InputProcessor.MouseWheelDelta(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_4:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 3));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_5:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 4));
                }
                break;
                default:
                    break;
            }

            return result;
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
