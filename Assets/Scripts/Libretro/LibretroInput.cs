using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private const int NUM_MAX_PLAYERS = 2;

        public class RetroPad
        {
            public readonly bool[] Buttons = new bool[Enum.GetValues(typeof(retro_device_id_joypad)).Length];
        }

        public RetroPad[] RetroPadButtons = new RetroPad[NUM_MAX_PLAYERS];

        private void RetroInputPollCallback()
        {
        }

        private short RetroInputStateCallback(uint port, retro_device device, uint index, uint id)
        {
            short result = 0;

            if (port < NUM_MAX_PLAYERS)
            {
                if (RetroPadButtons[port] == null)
                {
                    RetroPadButtons[port] = new RetroPad();
                }

                switch (device)
                {
                    case retro_device.RETRO_DEVICE_JOYPAD:
                    {
                        result = BoolToShort(RetroPadButtons[port].Buttons[id]);
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

        private short ProcessMouse(retro_device_id_mouse id)
        {
            short result = 0;

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                switch (id)
                {
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_X:
                        result = FloatToShort(mouse.delta.x.ReadValue());
                        break;
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_Y:
                        result = FloatToShort(-mouse.delta.y.ReadValue());
                        break;
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_4:
                    case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_5:
                    default:
                        break;
                }
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
