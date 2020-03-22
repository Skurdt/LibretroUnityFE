using SK.Utilities;
using System.Runtime.InteropServices;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        public class Core
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroInitDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroDeinitDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int RetroApiVersionDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroGetSystemInfoDelegate(ref RetroSystemInfo info);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroGetSystemAVInfoDelegate(ref RetroSystemAVInfo info);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetEnvironmentDelegate(RetroEnvironmentDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetVideoRefreshDelegate(RetroVideoRefreshDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetAudioSampleDelegate(RetroAudioSampleDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetAudioSampleBatchDelegate(RetroAudioSampleBatchDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetInputPollDelegate(RetroInputPollDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetInputStateDelegate(RetroInputStateDelegate cb);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroSetControllerPortDeviceDelegate(uint port, uint device);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroResetDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroRunDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate uint RetroSerializeSizeDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe delegate bool RetroSerializeDelegate(void* data, uint size);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe delegate bool RetroUnserializeDelegate(void* data, uint size);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroCheatResetDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public unsafe delegate void RetroCheatSetDelegate(uint index, [MarshalAs(UnmanagedType.U1)] bool enabled, char* code);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public delegate bool RetroLoadGameDelegate(ref RetroGameInfo game);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public delegate bool RetroLoadGameSpecialDelegate(uint game_type, ref RetroGameInfo info, uint num_info);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RetroUnloadGameDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate uint RetroGetRegionDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public unsafe delegate void* RetroGetMemoryDataDelegate(RetroMemory id);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate uint RetroGetMemorySizeDelegate(RetroMemory id);

            public RetroInitDelegate RetroInit;
            public RetroDeinitDelegate RetroDeinit;
            public RetroApiVersionDelegate RetroApiVersion;
            public RetroGetSystemInfoDelegate RetroGetSystemInfo;
            public RetroGetSystemAVInfoDelegate RetroGetSystemAVInfo;
            public RetroSetEnvironmentDelegate RetroSetEnvironment;
            public RetroSetVideoRefreshDelegate RetroSetVideoRefresh;
            public RetroSetAudioSampleDelegate RetroSetAudioSample;
            public RetroSetAudioSampleBatchDelegate RetroSetAudioSampleBatch;
            public RetroSetInputPollDelegate RetroSetInputPoll;
            public RetroSetInputStateDelegate RetroSetInputState;
            public RetroSetControllerPortDeviceDelegate RetroSetControllerPortDevice;
            public RetroResetDelegate RetroReset;
            public RetroRunDelegate RetroRun;
            public RetroSerializeSizeDelegate RetroSerializeSize;
            public RetroSerializeDelegate RetroSerialize;
            public RetroUnserializeDelegate RetroUnserialize;
            public RetroCheatResetDelegate RetroCheatReset;
            public RetroCheatSetDelegate RetroCheatSet;
            public RetroLoadGameDelegate RetroLoadGame;
            public RetroLoadGameSpecialDelegate RetroLoadGameSpecial;
            public RetroUnloadGameDelegate RetroUnloadGame;
            public RetroGetRegionDelegate RetroGetRegion;
            public RetroGetMemoryDataDelegate RetroGetMemoryData;
            public RetroGetMemorySizeDelegate RetroGetMemorySize;

            public unsafe struct SubSystem
            {
                public RetroSubsystemInfo* data;
                public uint size;
            }

            public unsafe struct Ports
            {
                public RetroControllerInfo* data;
                public uint size;
            }

            //public uint poll_type;
            //public bool inited;
            //public bool symbols_inited;
            //public bool game_loaded;
            //public bool input_polled;
            //public bool has_set_subsystems;
            //public bool has_set_input_descriptors;
            //ulong serialization_quirks_v;

            //public RetroSystemInfo info;

            public uint rotation;
            //public uint performance_level;
            //public bool load_no_content;

            //string input_desc_btn[MAX_USERS][RARCH_FIRST_META_KEY];
            //public string valid_extensions;

            //public bool supports_vfs;

            //disk_control_interface_t disk_control;
            //retro_location_callback location_cb;

            //public SubSystem subsystem;

            //public Ports ports;

            // rarch_memory_map_t mmaps;

            private DllModule _dll;

            public Core(string corePath)
            {
                try
                {
                    _dll = new DllModuleWindows(corePath);
                    GetCoreFunctions(_dll);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
            }

            public void DeInit()
            {
                try
                {
                    _dll.Free();
                    _dll = null;
                }
                catch (System.Exception e)
                {
                    Log.Exception(e.Message);
                }
            }

            private void GetCoreFunctions(DllModule core)
            {
                RetroInit = core.GetFunction<RetroInitDelegate>("retro_init");
                RetroDeinit = core.GetFunction<RetroDeinitDelegate>("retro_deinit");
                RetroApiVersion = core.GetFunction<RetroApiVersionDelegate>("retro_api_version");
                RetroGetSystemInfo = core.GetFunction<RetroGetSystemInfoDelegate>("retro_get_system_info");
                RetroGetSystemAVInfo = core.GetFunction<RetroGetSystemAVInfoDelegate>("retro_get_system_av_info");
                RetroSetEnvironment = core.GetFunction<RetroSetEnvironmentDelegate>("retro_set_environment");
                RetroSetVideoRefresh = core.GetFunction<RetroSetVideoRefreshDelegate>("retro_set_video_refresh");
                RetroSetAudioSample = core.GetFunction<RetroSetAudioSampleDelegate>("retro_set_audio_sample");
                RetroSetAudioSampleBatch = core.GetFunction<RetroSetAudioSampleBatchDelegate>("retro_set_audio_sample_batch");
                RetroSetInputPoll = core.GetFunction<RetroSetInputPollDelegate>("retro_set_input_poll");
                RetroSetInputState = core.GetFunction<RetroSetInputStateDelegate>("retro_set_input_state");
                RetroSetControllerPortDevice = core.GetFunction<RetroSetControllerPortDeviceDelegate>("retro_set_controller_port_device");
                RetroReset = core.GetFunction<RetroResetDelegate>("retro_reset");
                RetroRun = core.GetFunction<RetroRunDelegate>("retro_run");
                RetroSerializeSize = core.GetFunction<RetroSerializeSizeDelegate>("retro_serialize_size");
                RetroSerialize = core.GetFunction<RetroSerializeDelegate>("retro_serialize");
                RetroUnserialize = core.GetFunction<RetroUnserializeDelegate>("retro_unserialize");
                RetroCheatReset = core.GetFunction<RetroCheatResetDelegate>("retro_cheat_reset");
                RetroCheatSet = core.GetFunction<RetroCheatSetDelegate>("retro_cheat_set");
                RetroLoadGame = core.GetFunction<RetroLoadGameDelegate>("retro_load_game");
                RetroLoadGameSpecial = core.GetFunction<RetroLoadGameSpecialDelegate>("retro_load_game_special");
                RetroUnloadGame = core.GetFunction<RetroUnloadGameDelegate>("retro_unload_game");
                RetroGetRegion = core.GetFunction<RetroGetRegionDelegate>("retro_get_region");
                RetroGetMemoryData = core.GetFunction<RetroGetMemoryDataDelegate>("retro_get_memory_data");
                RetroGetMemorySize = core.GetFunction<RetroGetMemorySizeDelegate>("retro_get_memory_size");
            }
        }
    }
}
