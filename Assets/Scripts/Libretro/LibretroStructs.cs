using System;
using System.Runtime.InteropServices;

using retro_usec_t = System.Int64;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private struct retro_vfs_file_handle { }
        private struct retro_vfs_dir_handle { }

        [StructLayout(LayoutKind.Sequential)]
        private class retro_vfs_interface
        {
            public retro_vfs_get_path_t get_path;
            public retro_vfs_open_t open;
            public retro_vfs_close_t close;
            public retro_vfs_size_t size;
            public retro_vfs_tell_t tell;
            public retro_vfs_seek_t seek;
            public retro_vfs_read_t read;
            public retro_vfs_write_t write;
            public retro_vfs_flush_t flush;
            public retro_vfs_remove_t remove;
            public retro_vfs_rename_t rename;
            public retro_vfs_truncate_t truncate;
            public retro_vfs_stat_t stat;
            public retro_vfs_mkdir_t mkdir;
            public retro_vfs_opendir_t opendir;
            public retro_vfs_readdir_t readdir;
            public retro_vfs_dirent_get_name_t dirent_get_name;
            public retro_vfs_dirent_is_dir_t dirent_is_dir;
            public retro_vfs_closedir_t closedir;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_vfs_interface_info
        {
            public uint required_interface_version;
            public retro_vfs_interface iface;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_interface
        {
            public retro_hw_render_interface_type interface_type;
            public uint interface_version;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_led_interface
        {
            public retro_set_led_state_t set_led_state;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_midi_interface
        {
            public retro_midi_input_enabled_t input_enabled;
            public retro_midi_output_enabled_t output_enabled;
            public retro_midi_read_t read;
            public retro_midi_write_t write;
            public retro_midi_flush_t flush;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_context_negotiation_interface
        {
            public retro_hw_render_context_negotiation_interface_type interface_type;
            public uint interface_version;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_memory_descriptor
        {
            public ulong flags;
            public void* ptr;
            public ulong offset;
            public ulong start;
            public ulong select;
            public ulong disconnect;
            public ulong len;
            public char* addrspace;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_memory_map
        {
            public retro_memory_descriptor* descriptors;
            public uint num_descriptors;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_controller_description
        {
            public char* desc;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_controller_info
        {
            public retro_controller_description* types;
            public uint num_types;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_memory_info
        {
            public char* extension;
            public uint type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_rom_info
        {
            public char* desc;
            public char* valid_extensions;
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;
            [MarshalAs(UnmanagedType.U1)] public bool required;
            public retro_subsystem_memory_info* memory;
            public uint num_memory;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_info
        {
            public char* desc;
            public char* ident;
            public retro_subsystem_rom_info* roms;
            public uint num_roms;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_get_proc_address_interface
        {
            public retro_get_proc_address_t get_proc_address;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_log_callback
        {
            public IntPtr log;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_perf_counter
        {
            public char* ident;
            public ulong start;
            public ulong total;
            public ulong call_cnt;

            [MarshalAs(UnmanagedType.U1)] public bool registered;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_perf_callback
        {
            public retro_perf_get_time_usec_t get_time_usec;
            public retro_get_cpu_features_t get_cpu_features;

            public retro_perf_get_counter_t get_perf_counter;
            public retro_perf_register_t perf_register;
            public retro_perf_start_t perf_start;
            public retro_perf_stop_t perf_stop;
            public retro_perf_log_t perf_log;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_camera_callback
        {
            public ulong caps;
            public uint width;
            public uint height;

            public retro_camera_start_t start;
            public retro_camera_stop_t stop;
            public retro_camera_frame_raw_framebuffer_t frame_raw_framebuffer;
            public retro_camera_frame_opengl_texture_t frame_opengl_texture;
            public retro_camera_lifetime_status_t initialized;
            public retro_camera_lifetime_status_t deinitialized;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_location_callback
        {
            public retro_location_start_t start;
            public retro_location_stop_t stop;
            public retro_location_get_position_t get_position;
            public retro_location_set_interval_t set_interval;

            public retro_location_lifetime_status_t initialized;
            public retro_location_lifetime_status_t deinitialized;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_rumble_interface
        {
            public retro_set_rumble_state_t set_rumble_state;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_audio_callback
        {
            public retro_audio_callback_t callback;
            public retro_audio_set_state_callback_t set_state;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_frame_time_callback
        {
            public retro_frame_time_callback_t callback;
            public retro_usec_t reference;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_callback
        {
            public retro_hw_context_type context_type;
            public retro_hw_context_reset_t context_reset;
            public retro_hw_get_current_framebuffer_t get_current_framebuffer;
            public retro_hw_get_proc_address_t get_proc_address;
            public bool depth;
            public bool stencil;
            public bool bottom_left_origin;
            public uint version_major;
            public uint version_minor;
            public bool cache_context;

            public retro_hw_context_reset_t context_destroy;

            public bool debug_context;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_keyboard_callback
        {
            public retro_keyboard_event_t callback;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_disk_control_callback
        {
            public retro_set_eject_state_t set_eject_state;
            public retro_get_eject_state_t get_eject_state;

            public retro_get_image_index_t get_image_index;
            public retro_set_image_index_t set_image_index;
            public retro_get_num_images_t get_num_images;

            public retro_replace_image_index_t replace_image_index;
            public retro_add_image_index_t add_image_index;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_disk_control_ext_callback
        {
            public retro_set_eject_state_t set_eject_state;
            public retro_get_eject_state_t get_eject_state;

            public retro_get_image_index_t get_image_index;
            public retro_set_image_index_t set_image_index;
            public retro_get_num_images_t get_num_images;

            public retro_replace_image_index_t replace_image_index;
            public retro_add_image_index_t add_image_index;

            public retro_set_initial_image_t set_initial_image;

            public retro_get_image_path_t get_image_path;
            public retro_get_image_label_t get_image_label;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_message
        {
            public char* msg;
            public uint frames;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_input_descriptor
        {
            public uint port;
            public uint device;
            public uint index;
            public uint id;
            public char* desc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_system_info
        {
            public char* library_name;
            public char* library_version;
            public char* valid_extensions;
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_game_geometry
        {
            public uint base_width;
            public uint base_height;
            public uint max_width;
            public uint max_height;
            public float aspect_ratio;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_timing
        {
            public double fps;
            public double sample_rate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_av_info
        {
            public retro_game_geometry geometry;
            public retro_system_timing timing;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_variable
        {
            public char* key;
            public char* value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_display
        {
            public char* key;
            [MarshalAs(UnmanagedType.U1)] public bool visible;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_value
        {
            public char* value;
            public char* label;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_definition
        {
            public char* key;
            public char* desc;
            public char* info;
            public retro_core_option_value[] values;
            public char* default_value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_core_options_intl
        {
            public retro_core_option_definition[] us;
            public retro_core_option_definition[] local;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_game_info
        {
            public char* path;
            public void* data;
            public uint size;
            public char* meta;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_framebuffer
        {
            public IntPtr data;
            public uint width;
            public uint height;
            public uint pitch;
            public retro_pixel_format format;
            public uint access_flags;
            public uint memory_flags;
        }
    }
}
