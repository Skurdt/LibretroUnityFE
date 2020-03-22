using SK.Utilities;
using System;
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
            bool key = false;

            if (port == 0)
            {
                switch (device)
                {
                    case RetroDevice.RETRO_DEVICE_JOYPAD:
                    {
                        key = ProcessJoypad(id);
                    }
                    break;
                    case RetroDevice.RETRO_DEVICE_MOUSE:
                    {
                        key = ProcessMouse(id);
                    }
                    break;
                    case RetroDevice.RETRO_DEVICE_KEYBOARD:
                    {
                        key = ProcessKeyboard(id);
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

            return (short)(key ? 1 : 0);
        }

        private static bool ProcessJoypad(uint id)
        {
            bool result = false;

            RetroPad retroPad = (RetroPad)id;
            switch (retroPad)
            {
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_B:
                    result = Input.GetKey(KeyCode.Keypad2);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_Y:
                    result = Input.GetKey(KeyCode.Keypad5);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_SELECT:
                    result = Input.GetKey(KeyCode.RightShift);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_START:
                    result = Input.GetKey(KeyCode.Return);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_UP:
                    result = Input.GetKey(KeyCode.Z);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_DOWN:
                    result = Input.GetKey(KeyCode.S);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_LEFT:
                    result = Input.GetKey(KeyCode.Q);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_RIGHT:
                    result = Input.GetKey(KeyCode.D);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_A:
                    result = Input.GetKey(KeyCode.Keypad1);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_X:
                    result = Input.GetKey(KeyCode.Keypad4);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L:
                    result = Input.GetKey(KeyCode.Keypad6);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R:
                    result = Input.GetKey(KeyCode.Keypad3);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L2:
                    result = Input.GetKey(KeyCode.W);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R2:
                    result = Input.GetKey(KeyCode.X);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_L3:
                    result = Input.GetKey(KeyCode.C);
                    break;
                case RetroPad.RETRO_DEVICE_ID_JOYPAD_R3:
                    result = Input.GetKey(KeyCode.V);
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool ProcessMouse(uint id)
        {
            bool result = false;

            RetroMouse mouseId = (RetroMouse)id;
            switch (mouseId)
            {
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_X:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_Y:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                    result = Input.GetMouseButtonDown(0);
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                    result = Input.GetMouseButtonDown(1);
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                    break;
                case RetroMouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                    result = Input.GetMouseButtonDown(2);
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

        private bool ProcessKeyboard(uint id)
        {
            return Input.GetKey((KeyCode)id);
        }
    }
}
