/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using SK.Libretro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.LibretroDelegates;
using static SK.Libretro.LibretroStructs;

namespace SK.Libretro
{
    public sealed class LibretroCore
    {
        #region Dynamically loaded function pointers
        internal retro_set_environment_t retro_set_environment;
        internal retro_set_video_refresh_t retro_set_video_refresh;
        internal retro_set_audio_sample_t retro_set_audio_sample;
        internal retro_set_audio_sample_batch_t retro_set_audio_sample_batch;
        internal retro_set_input_poll_t retro_set_input_poll;
        internal retro_set_input_state_t retro_set_input_state;

        public retro_init_t retro_init;
        internal retro_deinit_t retro_deinit;
        internal retro_api_version_t retro_api_version;
        internal retro_get_system_info_t retro_get_system_info;
        internal retro_get_system_av_info_t retro_get_system_av_info;
        internal retro_set_controller_port_device_t retro_set_controller_port_device;
        internal retro_reset_t retro_reset;
        public retro_run_t retro_run;
        internal retro_serialize_size_t retro_serialize_size;
        internal retro_serialize_t retro_serialize;
        internal retro_unserialize_t retro_unserialize;
        internal retro_cheat_reset_t retro_cheat_reset;
        internal retro_cheat_set_t retro_cheat_set;
        internal retro_load_game_t retro_load_game;
        internal retro_load_game_special_t retro_load_game_special;
        internal retro_unload_game_t retro_unload_game;
        internal retro_get_region_t retro_get_region;
        internal retro_get_memory_data_t retro_get_memory_data;
        internal retro_get_memory_size_t retro_get_memory_size;
        #endregion

        internal bool Initialized { get; private set; } = false;

        internal int ApiVersion { get; private set; }

        internal string CoreName { get; private set; }
        internal string CoreLibraryName { get; private set; }
        internal string CoreLibraryVersion { get; private set; }
        internal string[] ValidExtensions { get; private set; }
        internal bool NeedFullPath { get; private set; }
        internal bool BlockExtract { get; private set; }

        public int Rotation { get; internal set; }

        public bool HwAccelerated { get; internal set; }

        internal int PerformanceLevel;
        internal bool SupportNoGame;

        internal LibretroCoreOptions CoreOptions;

        internal retro_controller_info[] ControllerPorts;

        private DllModule _dll;
        private readonly LibretroWrapper _wrapper;
        private readonly List<IntPtr> _unsafeStrings = new List<IntPtr>();

        internal LibretroCore(LibretroWrapper wrapper) => _wrapper = wrapper;

        internal unsafe bool Start(string coreName)
        {
            try
            {
                switch (_wrapper.TargetPlatform)
                {
                    case LibretroTargetPlatform.WindowsEditor:
                    case LibretroTargetPlatform.WindowsPlayer:
                    {
                        _dll = new DllModuleWindows();
                    }
                    break;
                    case LibretroTargetPlatform.OSXEditor:
                    case LibretroTargetPlatform.OSXPlayer:
                    {
                        _dll = new DllModuleOSX();
                    }
                    break;
                    case LibretroTargetPlatform.LinuxEditor:
                    case LibretroTargetPlatform.LinuxPlayer:
                    {
                        _dll = new DllModuleLinux();
                    }
                    break;
                    default:
                        Log.Error($"Target platform '{_wrapper.TargetPlatform}' not supported.");
                        return false;
                }

                string corePath = FileSystem.GetAbsolutePath($"{LibretroWrapper.CoresDirectory}/{coreName}_libretro.{_dll.Extension}");
                if (!FileSystem.FileExists(corePath))
                {
                    Log.Error($"Core '{coreName}' at path '{corePath}' not found.");
                    return false;
                }

                string tempDirectory = FileSystem.GetAbsolutePath(LibretroWrapper.TempDirectory);
                if (!Directory.Exists(tempDirectory))
                {
                    _ = Directory.CreateDirectory(tempDirectory);
                }

                string instancePath = Path.Combine(tempDirectory, $"{coreName}_{Guid.NewGuid()}.{_dll.Extension}");
                File.Copy(corePath, instancePath);

                _dll.Load(instancePath);

                GetCoreFunctions();

                ApiVersion = retro_api_version();

                retro_system_info systemInfo = new retro_system_info();
                retro_get_system_info(ref systemInfo);

                CoreName           = coreName;
                CoreLibraryName    = StringUtils.CharsToString(systemInfo.library_name);
                CoreLibraryVersion = StringUtils.CharsToString(systemInfo.library_version);
                if (systemInfo.valid_extensions != null)
                {
                    ValidExtensions = StringUtils.CharsToString(systemInfo.valid_extensions).Split('|');
                }
                NeedFullPath = systemInfo.need_fullpath;
                BlockExtract = systemInfo.block_extract;

                retro_set_environment(_wrapper.EnvironmentCallback);
                retro_set_video_refresh(_wrapper.VideoRefreshCallback);
                retro_set_audio_sample(_wrapper.AudioSampleCallback);
                retro_set_audio_sample_batch(_wrapper.AudioSampleBatchCallback);
                retro_set_input_poll(_wrapper.InputPollCallback);
                retro_set_input_state(_wrapper.InputStateCallback);
                retro_init();

                Initialized = true;
                return true;
            }
            catch (Exception e )
            {
                Log.Exception(e, "Libretro.LibretroCore.Start");
                Stop();
            }

            return false;
        }

        internal void Stop()
        {
            try
            {
                //FIXME(Tom): This sometimes crash (mostly on cores using lico)
                //if (Initialized)
                //{
                //    retro_deinit();
                //}

                if (_dll != null)
                {
                    _dll.Free();

                    string dllPath = FileSystem.GetAbsolutePath($"{LibretroWrapper.TempDirectory}/{_dll.Name}.{_dll.Extension}");
                    if (File.Exists(dllPath))
                    {
                        File.Delete(dllPath);
                    }

                    _dll = null;
                }

                for (int i = 0; i < _unsafeStrings.Count; ++i)
                {
                    Marshal.FreeHGlobal(_unsafeStrings[i]);
                }

                Initialized = false;
            }
            catch (Exception e)
            {
                Log.Exception(e, "Libretro.LibretroCore.Stop");
            }
        }

        internal unsafe char* GetUnsafeString(string source)
        {
            _unsafeStrings.Add(StringUtils.StringToChars(source, out char* result));
            return result;
        }

        private void GetCoreFunctions()
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
        }
    }
}
