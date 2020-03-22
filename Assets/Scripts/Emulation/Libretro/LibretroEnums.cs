namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        public enum RetroDevice
        {
            RETRO_DEVICE_NONE     = 0,
            RETRO_DEVICE_JOYPAD   = 1,
            RETRO_DEVICE_MOUSE    = 2,
            RETRO_DEVICE_KEYBOARD = 3,
            RETRO_DEVICE_LIGHTGUN = 4,
            RETRO_DEVICE_ANALOG   = 5,
            RETRO_DEVICE_POINTER  = 6
        }

        public enum RetroPad
        {
            RETRO_DEVICE_ID_JOYPAD_B      = 0,
            RETRO_DEVICE_ID_JOYPAD_Y      = 1,
            RETRO_DEVICE_ID_JOYPAD_SELECT = 2,
            RETRO_DEVICE_ID_JOYPAD_START  = 3,
            RETRO_DEVICE_ID_JOYPAD_UP     = 4,
            RETRO_DEVICE_ID_JOYPAD_DOWN   = 5,
            RETRO_DEVICE_ID_JOYPAD_LEFT   = 6,
            RETRO_DEVICE_ID_JOYPAD_RIGHT  = 7,
            RETRO_DEVICE_ID_JOYPAD_A      = 8,
            RETRO_DEVICE_ID_JOYPAD_X      = 9,
            RETRO_DEVICE_ID_JOYPAD_L      = 10,
            RETRO_DEVICE_ID_JOYPAD_R      = 11,
            RETRO_DEVICE_ID_JOYPAD_L2     = 12,
            RETRO_DEVICE_ID_JOYPAD_R2     = 13,
            RETRO_DEVICE_ID_JOYPAD_L3     = 14,
            RETRO_DEVICE_ID_JOYPAD_R3     = 15
        }

        public enum RetroAnalog
        {
            RETRO_DEVICE_INDEX_ANALOG_LEFT   = 0,
            RETRO_DEVICE_INDEX_ANALOG_RIGHT  = 1,
            RETRO_DEVICE_INDEX_ANALOG_BUTTON = 2,
            RETRO_DEVICE_ID_ANALOG_X         = 0,
            RETRO_DEVICE_ID_ANALOG_Y         = 1
        }

        public enum RetroMouse
        {
            RETRO_DEVICE_ID_MOUSE_X               = 0,
            RETRO_DEVICE_ID_MOUSE_Y               = 1,
            RETRO_DEVICE_ID_MOUSE_LEFT            = 2,
            RETRO_DEVICE_ID_MOUSE_RIGHT           = 3,
            RETRO_DEVICE_ID_MOUSE_WHEELUP         = 4,
            RETRO_DEVICE_ID_MOUSE_WHEELDOWN       = 5,
            RETRO_DEVICE_ID_MOUSE_MIDDLE          = 6,
            RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP   = 7,
            RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN = 8,
            RETRO_DEVICE_ID_MOUSE_BUTTON_4        = 9,
            RETRO_DEVICE_ID_MOUSE_BUTTON_5        = 10
        }

        public enum RetroKey
        {
            RETROK_UNKNOWN = 0,
            RETROK_FIRST = 0,
            RETROK_BACKSPACE = 8,
            RETROK_TAB = 9,
            RETROK_CLEAR = 12,
            RETROK_RETURN = 13,
            RETROK_PAUSE = 19,
            RETROK_ESCAPE = 27,
            RETROK_SPACE = 32,
            RETROK_EXCLAIM = 33,
            RETROK_QUOTEDBL = 34,
            RETROK_HASH = 35,
            RETROK_DOLLAR = 36,
            RETROK_AMPERSAND = 38,
            RETROK_QUOTE = 39,
            RETROK_LEFTPAREN = 40,
            RETROK_RIGHTPAREN = 41,
            RETROK_ASTERISK = 42,
            RETROK_PLUS = 43,
            RETROK_COMMA = 44,
            RETROK_MINUS = 45,
            RETROK_PERIOD = 46,
            RETROK_SLASH = 47,
            RETROK_0 = 48,
            RETROK_1 = 49,
            RETROK_2 = 50,
            RETROK_3 = 51,
            RETROK_4 = 52,
            RETROK_5 = 53,
            RETROK_6 = 54,
            RETROK_7 = 55,
            RETROK_8 = 56,
            RETROK_9 = 57,
            RETROK_COLON = 58,
            RETROK_SEMICOLON = 59,
            RETROK_LESS = 60,
            RETROK_EQUALS = 61,
            RETROK_GREATER = 62,
            RETROK_QUESTION = 63,
            RETROK_AT = 64,
            RETROK_LEFTBRACKET = 91,
            RETROK_BACKSLASH = 92,
            RETROK_RIGHTBRACKET = 93,
            RETROK_CARET = 94,
            RETROK_UNDERSCORE = 95,
            RETROK_BACKQUOTE = 96,
            RETROK_a = 97,
            RETROK_b = 98,
            RETROK_c = 99,
            RETROK_d = 100,
            RETROK_e = 101,
            RETROK_f = 102,
            RETROK_g = 103,
            RETROK_h = 104,
            RETROK_i = 105,
            RETROK_j = 106,
            RETROK_k = 107,
            RETROK_l = 108,
            RETROK_m = 109,
            RETROK_n = 110,
            RETROK_o = 111,
            RETROK_p = 112,
            RETROK_q = 113,
            RETROK_r = 114,
            RETROK_s = 115,
            RETROK_t = 116,
            RETROK_u = 117,
            RETROK_v = 118,
            RETROK_w = 119,
            RETROK_x = 120,
            RETROK_y = 121,
            RETROK_z = 122,
            RETROK_LEFTBRACE = 123,
            RETROK_BAR = 124,
            RETROK_RIGHTBRACE = 125,
            RETROK_TILDE = 126,
            RETROK_DELETE = 127,

            RETROK_KP0 = 256,
            RETROK_KP1 = 257,
            RETROK_KP2 = 258,
            RETROK_KP3 = 259,
            RETROK_KP4 = 260,
            RETROK_KP5 = 261,
            RETROK_KP6 = 262,
            RETROK_KP7 = 263,
            RETROK_KP8 = 264,
            RETROK_KP9 = 265,
            RETROK_KP_PERIOD = 266,
            RETROK_KP_DIVIDE = 267,
            RETROK_KP_MULTIPLY = 268,
            RETROK_KP_MINUS = 269,
            RETROK_KP_PLUS = 270,
            RETROK_KP_ENTER = 271,
            RETROK_KP_EQUALS = 272,

            RETROK_UP = 273,
            RETROK_DOWN = 274,
            RETROK_RIGHT = 275,
            RETROK_LEFT = 276,
            RETROK_INSERT = 277,
            RETROK_HOME = 278,
            RETROK_END = 279,
            RETROK_PAGEUP = 280,
            RETROK_PAGEDOWN = 281,

            RETROK_F1 = 282,
            RETROK_F2 = 283,
            RETROK_F3 = 284,
            RETROK_F4 = 285,
            RETROK_F5 = 286,
            RETROK_F6 = 287,
            RETROK_F7 = 288,
            RETROK_F8 = 289,
            RETROK_F9 = 290,
            RETROK_F10 = 291,
            RETROK_F11 = 292,
            RETROK_F12 = 293,
            RETROK_F13 = 294,
            RETROK_F14 = 295,
            RETROK_F15 = 296,

            RETROK_NUMLOCK = 300,
            RETROK_CAPSLOCK = 301,
            RETROK_SCROLLOCK = 302,
            RETROK_RSHIFT = 303,
            RETROK_LSHIFT = 304,
            RETROK_RCTRL = 305,
            RETROK_LCTRL = 306,
            RETROK_RALT = 307,
            RETROK_LALT = 308,
            RETROK_RMETA = 309,
            RETROK_LMETA = 310,
            RETROK_LSUPER = 311,
            RETROK_RSUPER = 312,
            RETROK_MODE = 313,
            RETROK_COMPOSE = 314,

            RETROK_HELP = 315,
            RETROK_PRINT = 316,
            RETROK_SYSREQ = 317,
            RETROK_BREAK = 318,
            RETROK_MENU = 319,
            RETROK_POWER = 320,
            RETROK_EURO = 321,
            RETROK_UNDO = 322,
            RETROK_OEM_102 = 323,

            RETROK_LAST,

            RETROK_DUMMY = int.MaxValue
        }

        public enum RetroMod
        {
            RETROKMOD_NONE = 0x0000,

            RETROKMOD_SHIFT = 0x01,
            RETROKMOD_CTRL  = 0x02,
            RETROKMOD_ALT   = 0x04,
            RETROKMOD_META  = 0x08,

            RETROKMOD_NUMLOCK   = 0x10,
            RETROKMOD_CAPSLOCK  = 0x20,
            RETROKMOD_SCROLLOCK = 0x40,

            RETROKMOD_DUMMY = int.MaxValue
        }

        public enum RetroLogLevel
        {
            RETRO_LOG_DEBUG = 0,
            RETRO_LOG_INFO  = 1,
            RETRO_LOG_WARN  = 2,
            RETRO_LOG_ERROR = 3,

            RETRO_LOG_DUMMY = int.MaxValue
        }

        public enum RetroRumbleEffect
        {
            RETRO_RUMBLE_STRONG = 0,
            RETRO_RUMBLE_WEAK   = 1,

            RETRO_RUMBLE_DUMMY = int.MaxValue
        }

        public enum RetroPixelFormat
        {
            /* 0RGB1555, native endian.
             * 0 bit must be set to 0.
             * This pixel format is default for compatibility concerns only.
             * If a 15/16-bit pixel format is desired, consider using RGB565. */
            RETRO_PIXEL_FORMAT_0RGB1555 = 0,

            /* XRGB8888, native endian.
             * X bits are ignored. */
            RETRO_PIXEL_FORMAT_XRGB8888 = 1,

            /* RGB565, native endian.
             * This pixel format is the recommended format to use if a 15/16-bit
             * format is desired as it is the pixel format that is typically
             * available on a wide range of low-power devices.
             *
             * It is also natively supported in APIs like OpenGL ES. */
            RETRO_PIXEL_FORMAT_RGB565 = 2,

            /* Ensure sizeof() == sizeof(int). */
            RETRO_PIXEL_FORMAT_UNKNOWN = int.MaxValue
        }

        public enum RetroMemory
        {
            SAVE_RAM = 0,
            RTC = 1,
            SYSTEM_RAM = 2,
            VIDEO_RAM = 3,

            DUMMY = int.MaxValue
        }

        public enum RetroEnvironment
        {
            EXPERIMENTAL = 0x10000,
            SET_ROTATION = 1,
            GET_OVERSCAN = 2,
            GET_CAN_DUPE = 3,
            SET_MESSAGE = 6,
            SHUTDOWN = 7,
            SET_PERFORMANCE_LEVEL = 8,
            GET_SYSTEM_DIRECTORY = 9,
            SET_PIXEL_FORMAT = 10,
            SET_INPUT_DESCRIPTORS = 11,
            SET_KEYBOARD_CALLBACK = 12,
            SET_DISK_CONTROL_INTERFACE = 13,
            SET_HW_RENDER = 14,
            GET_VARIABLE = 15,
            SET_VARIABLES = 16,
            GET_VARIABLE_UPDATE = 17,
            SET_SUPPORT_NO_GAME = 18,
            GET_LIBRETRO_PATH = 19,
            SET_FRAME_TIME_CALLBACK = 21,
            SET_AUDIO_CALLBACK = 22,
            GET_RUMBLE_INTERFACE = 23,
            GET_INPUT_DEVICE_CAPABILITIES = 24,
            GET_SENSOR_INTERFACE = 25 | EXPERIMENTAL,
            GET_CAMERA_INTERFACE = 26 | EXPERIMENTAL,
            GET_LOG_INTERFACE = 27,
            GET_PERF_INTERFACE = 28,
            GET_LOCATION_INTERFACE = 29,
            GET_CONTENT_DIRECTORY = 30,
            GET_CORE_ASSETS_DIRECTORY = 30,
            GET_SAVE_DIRECTORY = 31,
            SET_SYSTEM_AV_INFO = 32,
            SET_PROC_ADDRESS_CALLBACK = 33,
            SET_SUBSYSTEM_INFO = 34,
            SET_CONTROLLER_INFO = 35,
            SET_MEMORY_MAPS = 36 | EXPERIMENTAL,
            SET_GEOMETRY = 37,
            GET_USERNAME = 38,
            GET_LANGUAGE = 39,
            GET_CURRENT_SOFTWARE_FRAMEBUFFER = 40 | EXPERIMENTAL,
            GET_HW_RENDER_INTERFACE = 41 | EXPERIMENTAL,
            SET_SUPPORT_ACHIEVEMENTS = 42 | EXPERIMENTAL,
            SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE = 43 | EXPERIMENTAL,
            SET_SERIALIZATION_QUIRKS = 44,
            SET_HW_SHARED_CONTEXT = 44 | EXPERIMENTAL,
            GET_VFS_INTERFACE = 45 | EXPERIMENTAL,
            GET_LED_INTERFACE = 46 | EXPERIMENTAL,
            GET_AUDIO_VIDEO_ENABLE = 47 | EXPERIMENTAL,
            GET_MIDI_INTERFACE = 48 | EXPERIMENTAL,
            GET_FASTFORWARDING = 49 | EXPERIMENTAL,
            GET_TARGET_REFRESH_RATE = 50 | EXPERIMENTAL,
            GET_INPUT_BITMASKS = 51 | EXPERIMENTAL,
            GET_CORE_OPTIONS_VERSION = 52,
            SET_CORE_OPTIONS = 53,
            SET_CORE_OPTIONS_INTL = 54,
            SET_CORE_OPTIONS_DISPLAY = 55,
            GET_PREFERRED_HW_RENDER = 56,
            GET_DISK_CONTROL_INTERFACE_VERSION = 57,
            SET_DISK_CONTROL_EXT_INTERFACE = 58
        }
    }
}
