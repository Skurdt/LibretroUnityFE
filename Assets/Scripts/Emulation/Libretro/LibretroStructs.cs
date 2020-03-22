using System;
using System.Runtime.InteropServices;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RetroLedInterface
        {
            public IntPtr set_led_state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroControllerDescription
        {
            /* Human-readable description of the controller. Even if using a generic
             * input device type, this can be set to the particular device type the
             * core uses. */
            public char* desc;

            /* Device type passed to retro_set_controller_port_device(). If the device
             * type is a sub-class of a generic input device type, use the
             * RETRO_DEVICE_SUBCLASS macro to create an ID.
             *
             * E.g. RETRO_DEVICE_SUBCLASS(RETRO_DEVICE_JOYPAD, 1). */
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroControllerInfo
        {
            public RetroControllerDescription* types;
            public uint num_types;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroSubsystemMemoryInfo
        {
            /* The extension associated with a memory type, e.g. "psram". */
            public char* extension;

            /* The memory type for retro_get_memory(). This should be at
             * least 0x100 to avoid conflict with standardized
             * libretro memory types. */
            public uint type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroSubsystemRomInfo
        {
            /* Describes what the content is (SGB BIOS, GB ROM, etc). */
            public char* desc;

            /* Same definition as retro_get_system_info(). */
            public char* valid_extensions;

            /* Same definition as retro_get_system_info(). */
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;

            /* Same definition as retro_get_system_info(). */
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;

            /* This is set if the content is required to load a game.
             * If this is set to false, a zeroed-out retro_game_info can be passed. */
            [MarshalAs(UnmanagedType.U1)] public bool required;

            /* Content can have multiple associated persistent
             * memory types (retro_get_memory()). */
            public RetroSubsystemMemoryInfo* memory;
            public uint num_memory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroSubsystemInfo
        {
            /* Human-readable string of the subsystem type, e.g. "Super GameBoy" */
            public char* desc;

            /* A computer friendly short string identifier for the subsystem type.
             * This name must be [a-z].
             * E.g. if desc is "Super GameBoy", this can be "sgb".
             * This identifier can be used for command-line interfaces, etc.
             */
            public char* ident;

            /* Infos for each content file. The first entry is assumed to be the
             * "most significant" content for frontend purposes.
             * E.g. with Super GameBoy, the first content should be the GameBoy ROM,
             * as it is the most "significant" content to a user.
             * If a frontend creates new file paths based on the content used
             * (e.g. savestates), it should use the path for the first ROM to do so. */
            public RetroSubsystemRomInfo* roms;

            /* Number of content files associated with a subsystem. */
            public uint num_roms;

            /* The type passed to retro_load_game_special(). */
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroPerfCounter
        {
            public char* ident;
            public ulong start;
            public ulong total;
            public ulong call_cnt;

            [MarshalAs(UnmanagedType.U1)] public bool registered;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroMessage
        {
            public char* msg;   // Message to be displayed.
            public uint frames; // Duration in frames of message.
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroInputDescriptor
        {
            // Associates given parameters with a description.
            public uint port;
            public uint device;
            public uint index;
            public uint id;

            // Human readable description for parameters. The pointer must remain valid until retro_unload_game() is called.
            public char* desc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroSystemInfo
        {
            public char* library_name;
            public char* library_version;
            public char* valid_extensions;
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RetroGameGeometry
        {
            public uint base_width;
            public uint base_height;
            public uint max_width;
            public uint max_height;
            public float aspect_ratio;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RetroSystemTiming
        {
            public double fps;
            public double sample_rate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RetroSystemAVInfo
        {
            public RetroGameGeometry geometry;
            public RetroSystemTiming timing;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroVariable
        {
            public char* key;
            public char* value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroCoreOptionValue
        {
            /* Expected option value */
            public char* value;

            /* Human-readable value label. If NULL, value itself
             * will be displayed by the frontend */
            public char* label;
        }

        public const int RETRO_NUM_CORE_OPTION_VALUES_MAX = 128;

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroCoreOptionDefinition
        {
            /* Variable to query in RETRO_ENVIRONMENT_GET_VARIABLE. */
            public char* key;

            /* Human-readable core option description (used as menu label) */
            public char* desc;

            /* Human-readable core option information (used as menu sublabel) */
            public char* info;

            /* Array of retro_core_option_value structs, terminated by NULL */
            public RetroCoreOptionValue* values;

            /* Default core option value. Must match one of the values
             * in the retro_core_option_value array, otherwise will be
             * ignored */
            public char* default_value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroCoreOptionsIntl
        {
            /* Pointer to an array of retro_core_option_definition structs
             * - US English implementation
             * - Must point to a valid array */
            public RetroCoreOptionDefinition* us;

            /* Pointer to an array of retro_core_option_definition structs
             * - Implementation for current frontend language
             * - May be NULL */
            public RetroCoreOptionDefinition* local;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RetroGameInfo
        {
            public char* path;
            public void* data;
            public uint size;
            public char* meta;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RetroLogCallback
        {
            public IntPtr log;
        }
    }
}
