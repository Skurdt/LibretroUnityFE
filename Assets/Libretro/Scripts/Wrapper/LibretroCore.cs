using SK.Libretro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.Wrapper;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    [Serializable]
    public class CoreOptions
    {
        public string CoreName = string.Empty;
        public List<string> Options = new List<string>();
    }

    [Serializable]
    public class CoreOptionsList
    {
        public List<CoreOptions> Cores = new List<CoreOptions>();
    }

    public class LibretroCore
    {
        #region Dynamically loaded function pointers
        public retro_set_environment_t retro_set_environment;
        public retro_set_video_refresh_t retro_set_video_refresh;
        public retro_set_audio_sample_t retro_set_audio_sample;
        public retro_set_audio_sample_batch_t retro_set_audio_sample_batch;
        public retro_set_input_poll_t retro_set_input_poll;
        public retro_set_input_state_t retro_set_input_state;
        public retro_init_t retro_init;
        public retro_deinit_t retro_deinit;
        public retro_api_version_t retro_api_version;
        public retro_get_system_info_t retro_get_system_info;
        public retro_get_system_av_info_t retro_get_system_av_info;
        public retro_set_controller_port_device_t retro_set_controller_port_device;
        public retro_reset_t retro_reset;
        public retro_run_t retro_run;
        public retro_serialize_size_t retro_serialize_size;
        public retro_serialize_t retro_serialize;
        public retro_unserialize_t retro_unserialize;
        public retro_cheat_reset_t retro_cheat_reset;
        public retro_cheat_set_t retro_cheat_set;
        public retro_load_game_t retro_load_game;
        public retro_load_game_special_t retro_load_game_special;
        public retro_unload_game_t retro_unload_game;
        public retro_get_region_t retro_get_region;
        public retro_get_memory_data_t retro_get_memory_data;
        public retro_get_memory_size_t retro_get_memory_size;
        #endregion

        public bool Initialized { get; private set; }

        public int ApiVersion { get; private set; }

        public string CoreName { get; private set; }
        public string CoreVersion { get; private set; }
        public string[] ValidExtensions { get; private set; }
        public bool NeedFullPath { get; private set; }
        public bool BlockExtract { get; private set; }

        public int Rotation;
        public int PerformanceLevel;
        public bool SupportNoGame;

        public string[,] ButtonDescriptions = new string[MAX_USERS, FIRST_META_KEY];
        public bool HasInputDescriptors;

        public CoreOptions CoreOptions;

        public retro_controller_info[] ControllerPorts;

        private readonly DllModule _dll = new DllModuleWindows();

        private retro_environment_t _environmentCallback;
        private retro_video_refresh_t _videoRefreshCallback;
        private retro_audio_sample_t _audioSampleCallback;
        private retro_audio_sample_batch_t _audioSampleBatchCallback;
        private retro_input_poll_t _inputPollCallback;
        private retro_input_state_t _inputStateCallback;
        private retro_log_printf_t _logPrintfCallback;

        public unsafe bool Start(Wrapper wrapper, string coreName)
        {
            bool result = false;

            string corePath = FileSystem.GetAbsolutePath($"{CoresDirectory}/{coreName}_libretro.dll");
            if (FileSystem.FileExists(corePath))
            {
                string instancePath = FileSystem.GetAbsolutePath($"{TempDirectory}/{coreName}_{Guid.NewGuid()}.dll");
                File.Copy(corePath, instancePath);
                if (_dll.Load(instancePath))
                {
                    if (GetCoreFunctions())
                    {
                        try
                        {
                            ApiVersion = retro_api_version();

                            SetCallbacks(wrapper);

                            retro_system_info systemInfo = new retro_system_info();
                            retro_get_system_info(ref systemInfo);

                            CoreName = CharsToString(systemInfo.library_name);
                            CoreVersion = CharsToString(systemInfo.library_version);
                            ValidExtensions = CharsToString(systemInfo.valid_extensions).Split('|');
                            NeedFullPath = systemInfo.need_fullpath;
                            BlockExtract = systemInfo.block_extract;

                            retro_set_environment(_environmentCallback);
                            retro_set_video_refresh(_videoRefreshCallback);
                            retro_set_audio_sample(_audioSampleCallback);
                            retro_set_audio_sample_batch(_audioSampleBatchCallback);
                            retro_set_input_poll(_inputPollCallback);
                            retro_set_input_state(_inputStateCallback);
                            retro_init();

                            Initialized = true;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Log.Exception(e, "Libretro.LibretroCore.Start");
                        }
                    }
                    else
                    {
                        Log.Error("One of the function pointers couldn't be retrieved for core '{coreName}'.", "Libretro.LibretroCore.Start");
                    }
                }
                else
                {
                    Log.Error($"Failed to load library for core '{coreName}' .", "Libretro.LibretroCore.Start");
                }
            }
            else
            {
                Log.Error($"Core '{coreName}' at path '{corePath}' not found.", "Libretro.LibretroCore.Start");
            }

            return result;
        }

        public void Stop()
        {
            try
            {
                if (Initialized)
                {
                    retro_deinit();
                    Initialized = false;
                }

                _dll.Free();

                string dllPath = FileSystem.GetAbsolutePath($"{TempDirectory}/{_dll.Name}");
                if (File.Exists(dllPath))
                {
                    File.Delete(dllPath);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e, "Libretro.LibretroCore.Stop");
            }
        }

        private bool GetCoreFunctions()
        {
            bool result = false;

            try
            {
                retro_set_environment            = _dll.GetFunction<retro_set_environment_t>("retro_set_environment");
                retro_set_video_refresh          = _dll.GetFunction<retro_set_video_refresh_t>("retro_set_video_refresh");
                retro_set_audio_sample           = _dll.GetFunction<retro_set_audio_sample_t>("retro_set_audio_sample");
                retro_set_audio_sample_batch     = _dll.GetFunction<retro_set_audio_sample_batch_t>("retro_set_audio_sample_batch");
                retro_set_input_poll             = _dll.GetFunction<retro_set_input_poll_t>("retro_set_input_poll");
                retro_set_input_state            = _dll.GetFunction<retro_set_input_state_t>("retro_set_input_state");
                retro_init                       = _dll.GetFunction<retro_init_t>("retro_init");
                retro_deinit                     = _dll.GetFunction<retro_deinit_t>("retro_deinit");
                retro_api_version                = _dll.GetFunction<retro_api_version_t>("retro_api_version");
                retro_get_system_info            = _dll.GetFunction<retro_get_system_info_t>("retro_get_system_info");
                retro_get_system_av_info         = _dll.GetFunction<retro_get_system_av_info_t>("retro_get_system_av_info");
                retro_set_controller_port_device = _dll.GetFunction<retro_set_controller_port_device_t>("retro_set_controller_port_device");
                retro_reset                      = _dll.GetFunction<retro_reset_t>("retro_reset");
                retro_run                        = _dll.GetFunction<retro_run_t>("retro_run");
                retro_serialize_size             = _dll.GetFunction<retro_serialize_size_t>("retro_serialize_size");
                retro_serialize                  = _dll.GetFunction<retro_serialize_t>("retro_serialize");
                retro_unserialize                = _dll.GetFunction<retro_unserialize_t>("retro_unserialize");
                retro_cheat_reset                = _dll.GetFunction<retro_cheat_reset_t>("retro_cheat_reset");
                retro_cheat_set                  = _dll.GetFunction<retro_cheat_set_t>("retro_cheat_set");
                retro_load_game                  = _dll.GetFunction<retro_load_game_t>("retro_load_game");
                retro_load_game_special          = _dll.GetFunction<retro_load_game_special_t>("retro_load_game_special");
                retro_unload_game                = _dll.GetFunction<retro_unload_game_t>("retro_unload_game");
                retro_get_region                 = _dll.GetFunction<retro_get_region_t>("retro_get_region");
                retro_get_memory_data            = _dll.GetFunction<retro_get_memory_data_t>("retro_get_memory_data");
                retro_get_memory_size            = _dll.GetFunction<retro_get_memory_size_t>("retro_get_memory_size");

                result = true;
            }
            catch (Exception e)
            {
                Log.Exception(e, "Libretro.LibretroCore.GetCoreFunctions");
            }

            return result;
        }

        private unsafe void SetCallbacks(Wrapper wrapper)
        {
            _environmentCallback      = wrapper.RetroEnvironmentCallback;
            _videoRefreshCallback     = wrapper.RetroVideoRefreshCallback;
            _audioSampleCallback      = wrapper.RetroAudioSampleCallback;
            _audioSampleBatchCallback = wrapper.RetroAudioSampleBatchCallback;
            _inputPollCallback        = wrapper.RetroInputPollCallback;
            _inputStateCallback       = wrapper.RetroInputStateCallback;
            _logPrintfCallback        = wrapper.RetroLogPrintf;
        }

        public IntPtr SetLogCallback()
        {
            return Marshal.GetFunctionPointerForDelegate(_logPrintfCallback);
        }
    }
}
