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
using UnityEngine.InputSystem;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe bool RetroEnvironmentCallback(retro_environment cmd, void* data)
        {
            switch (cmd)
            {
                /************************************************************************************************
                / Data passed from the frontend to the core
                /***********************************************************************************************/
                case retro_environment.RETRO_ENVIRONMENT_GET_OVERSCAN:                                return GetOverscan(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_CAN_DUPE:                                return GetCanDupe(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:                        return GetSystemDirectory(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE:                                return GetVariable(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE:                         return GetVariableUpdate(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_LIBRETRO_PATH:                           return GetLibretroPath(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE:                        return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES:               return GetInputDeviceCapabilities(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE:                        return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE:                        return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_LOG_INTERFACE:                           return GetLogInterface(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_PERF_INTERFACE:                          return GetPerfInterface(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE:                      return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY:                   return GetCoreAssetsDirectory(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:                          return GetSaveDirectory(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_USERNAME:                                return GetUsername(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_LANGUAGE:                                return GetLanguage(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER:            return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE:                     return GetHwRenderInterface(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_VFS_INTERFACE:                           return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_LED_INTERFACE:                           return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE:                      return GetAudioVideoEnable(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_MIDI_INTERFACE:                          return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_FASTFORWARDING:                          return GetFastForwarding(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE:                     return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_BITMASKS:                          return false;
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION:                    return GetCoreOptionsVersion(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_PREFERRED_HW_RENDER:                     return GetPreferredHwRender(data);
                case retro_environment.RETRO_ENVIRONMENT_GET_DISK_CONTROL_INTERFACE_VERSION:          return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_MESSAGE_INTERFACE_VERSION:               return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_MAX_USERS:                         return ENVIRONMENT_NOT_IMPLEMENTED(cmd);

                /************************************************************************************************
                / Data passed from the core to the frontend
                /***********************************************************************************************/
                case retro_environment.RETRO_ENVIRONMENT_SET_ROTATION:                                return SetRotation(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_MESSAGE:                                 return SetMessage(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL:                       return SetPerformanceLevel(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:                            return SetPixelFormat(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS:                       return SetInputDescriptors(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK:                       return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE:                  return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER:                               return SetHwRender(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_VARIABLES:                               return SetVariables(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME:                         return SetSupportNoGame(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK:                     return SetFrameTimeCallback(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK:                          return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:                          return SetSystemAvInfo(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK:                   return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO:                          return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_CONTROLLER_INFO:                         return SetControllerInfo(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_MEMORY_MAPS:                             return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_GEOMETRY:                                return SetGeometry(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS:                    return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE: return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS:                    return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT:                       return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS:                            return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL:                       return SetCoreOptionsIntl(data);
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY:                    return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE:              return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_SHUTDOWN:                                    return Shutdown();
                case retro_environment.RETRO_ENVIRONMENT_SET_MESSAGE_EXT:                             return ENVIRONMENT_NOT_IMPLEMENTED(cmd);

                /************************************************************************************************
                / RetroArch Extensions
                /***********************************************************************************************/
                case retro_environment.RETRO_ENVIRONMENT_SET_SAVE_STATE_IN_BACKGROUND:                return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_GET_CLEAR_ALL_THREAD_WAITS_CB:               return ENVIRONMENT_NOT_IMPLEMENTED(cmd);
                case retro_environment.RETRO_ENVIRONMENT_POLL_TYPE_OVERRIDE:                          return ENVIRONMENT_NOT_IMPLEMENTED(cmd);

                default:
                {
                    Log.Error($"Environment unknown: {cmd}", "RetroEnvironmentCallback");
                    return false;
                }
            }
        }

        /************************************************************************************************
        / Temporary placeholder... hopefully...
        /***********************************************************************************************/
        private static bool ENVIRONMENT_NOT_IMPLEMENTED(retro_environment cmd)
        {
            Log.Error("Environment not implemented!", cmd.ToString());
            return false;
        }

        /************************************************************************************************
        / Data passed from the frontend to the core
        /***********************************************************************************************/
        #region FrontendToCore
        private unsafe bool GetOverscan(void* outOverscan)
        {
            *(bool*)outOverscan = !OptionCropOverscan;
            Log.Info($"-> Overscan: {!OptionCropOverscan}", "RETRO_ENVIRONMENT_GET_OVERSCAN");
            return true;
        }

        private static unsafe bool GetCanDupe(void* outCanDupe)
        {
            *(bool*)outCanDupe = true;
            Log.Info("-> CanDupe: true", "RETRO_ENVIRONMENT_GET_CAN_DUPE");
            return true;
        }

        private unsafe bool GetSystemDirectory(void* data)
        {
            string systemDirectory = FileSystem.GetAbsolutePath(Path.Combine(SystemDirectory, Core.CoreName));
            _unsafeStrings.Add(StringToChars(systemDirectory, out *(char**)data));
            Log.Info($"-> SystemDirectory: {systemDirectory}", "RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY");
            return true;
        }

        private unsafe bool GetVariable(void* data)
        {
            if (Core.CoreOptions == null)
            {
                Log.Warning($"Core didn't set its options.");
                return false;
            }

            retro_variable* outVariable = (retro_variable*)data;

            string key        = CharsToString(outVariable->key);
            string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
            if (coreOption == null)
            {
                Log.Warning($"Core option '{key}' not found.");
                return false;
            }

            _unsafeStrings.Add(StringToChars(coreOption.Split(';')[1], out outVariable->value));
            return true;
        }

        private unsafe bool GetVariableUpdate(void* data)
        {
            *(bool*)data    = _dirtyVariables;
            _dirtyVariables = false;
            return true;
        }

        private unsafe bool GetLibretroPath(void* data)
        {
            _unsafeStrings.Add(StringToChars(Path.GetFullPath(CoresDirectory), out *(char**)data));
            return true;
        }

        private unsafe bool GetInputDeviceCapabilities(void* data)
        {
            // TODO(Tom): Move queries to Unity layer
            ulong mask = 0;

            if (Keyboard.current != null)
            {
                mask |= (1 << (int)retro_device.RETRO_DEVICE_KEYBOARD);
            }

            if (Mouse.current != null)
            {
                mask |= (1 << (int)retro_device.RETRO_DEVICE_MOUSE);
            }

            if (Gamepad.current != null)
            {
                mask |= (1 << (int)retro_device.RETRO_DEVICE_JOYPAD);
            }

            *(ulong*)data = mask;
            return true;
        }

        private unsafe bool GetLogInterface(void* data)
        {
            ((retro_log_callback*)data)->log = Core.GetLogCallback();
            return true;
        }

        private unsafe bool GetPerfInterface(void* data)
        {
            retro_perf_callback* outPerfInterface = (retro_perf_callback*)data;
            outPerfInterface->get_time_usec       = Core.GetPerfGetTimeUsecCallback();
            outPerfInterface->get_cpu_features    = Core.GetGetCPUFeaturesCallback();
            outPerfInterface->get_perf_counter    = Core.GetPerfGetCounterCallback();
            outPerfInterface->perf_register       = Core.GetPerfRegisterCallback();
            outPerfInterface->perf_start          = Core.GetPerfStartCallback();
            outPerfInterface->perf_stop           = Core.GetPerfStopCallback();
            outPerfInterface->perf_log            = Core.GetPerfLogCallback();
            return true;
        }

        private unsafe bool GetCoreAssetsDirectory(void* data)
        {
            string coreAssetsDirectory = FileSystem.GetAbsolutePath(Path.Combine(CoreAssetsDirectory, Core.CoreName));
            _unsafeStrings.Add(StringToChars(coreAssetsDirectory, out *(char**)data));
            Log.Info($"-> CoreAssetsDirectory: {coreAssetsDirectory}", "RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY");
            return true;
        }

        private unsafe bool GetSaveDirectory(void* data)
        {
            string saveDirectory = FileSystem.GetAbsolutePath(Path.Combine(SavesDirectory, Core.CoreName));
            _unsafeStrings.Add(StringToChars(saveDirectory, out *(char**)data));
            Log.Info($"-> SaveDirectory: {saveDirectory}", "RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY");
            return true;
        }

        private unsafe bool GetUsername(void* data)
        {
            // TODO(Tom): Expose this
            _unsafeStrings.Add(StringToChars("default_user", out *(char**)data));
            return true;
        }

        private unsafe bool GetLanguage(void* data)
        {
            // TODO(Tom): Expose this
            *(uint*)data = (uint)retro_language.RETRO_LANGUAGE_ENGLISH;
            return true;
        }

        private unsafe bool GetHwRenderInterface(void* _/*data*/)
        {
            return false;
        }

        private static unsafe bool GetAudioVideoEnable(void* data)
        {
            int result = 0;
            result    |= 1; // if video enabled
            result    |= 2; // if audio enabled

            *(int*)data = result;
            return true;
        }

        private unsafe bool GetFastForwarding(void* data)
        {
            *(bool*)data = false;
            return true;
        }

        private static unsafe bool GetCoreOptionsVersion(void* data)
        {
            *(uint*)data = RETRO_API_VERSION;
            return true;
        }

        private unsafe bool GetPreferredHwRender(void* data)
        {
            if (_glSupport)
            {
                *(uint*)data = (uint)retro_hw_context_type.RETRO_HW_CONTEXT_OPENGL;
                return true;
            }

            *(uint*)data = (uint)retro_hw_context_type.RETRO_HW_CONTEXT_NONE;
            return false;
        }
        #endregion

        /************************************************************************************************
        / Data passed from the core to the frontend
        /***********************************************************************************************/
        #region CoreToFrontend
        private unsafe bool SetRotation(void* data)
        {
            // Values: 0,  1,   2,   3
            // Result: 0, 90, 180, 270 degrees
            Core.Rotation = (int)*(uint*)data * 90;
            Log.Info($"<- Rotation: {Core.Rotation}", "RETRO_ENVIRONMENT_SET_ROTATION");
            return true;
        }

        private static unsafe bool SetMessage(void* data)
        {
            // TODO(Tom): Do I need something from this?
            Log.Warning($"<- Message: {CharsToString(((retro_message*)data)->msg)}", "RETRO_ENVIRONMENT_SET_MESSAGE");
            return true;
        }

        private unsafe bool SetPerformanceLevel(void* data)
        {
            Core.PerformanceLevel = *(int*)data;
            Log.Info($"<- PerformanceLevel: {Core.PerformanceLevel}", "RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL");
            return true;
        }

        private unsafe bool SetPixelFormat(void* data)
        {
            retro_pixel_format* inPixelFormat = (retro_pixel_format*)data;
            switch (*inPixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                {
                    Game.PixelFormat = *inPixelFormat;
                    Log.Info($"<- PixelFormat: {*inPixelFormat}", "RETRO_ENVIRONMENT_SET_PIXEL_FORMAT");
                    return true;
                }
            }

            return false;
        }

        private unsafe bool SetInputDescriptors(void* data)
        {
            retro_input_descriptor* inInputDescriptors = (retro_input_descriptor*)data;
            uint id;
            for (; inInputDescriptors->desc != null; ++inInputDescriptors)
            {
                uint port = inInputDescriptors->port;
                if (port >= MAX_USERS)
                {
                    continue;
                }

                retro_device device = (retro_device)inInputDescriptors->device;
                if (device != retro_device.RETRO_DEVICE_JOYPAD && device != retro_device.RETRO_DEVICE_ANALOG)
                {
                    continue;
                }

                id = inInputDescriptors->id;
                if (id >= FIRST_CUSTOM_BIND)
                {
                    continue;
                }

                string descText = CharsToString(inInputDescriptors->desc);
                retro_device_index_analog index = (retro_device_index_analog)inInputDescriptors->index;
                if (device == retro_device.RETRO_DEVICE_ANALOG)
                {
                    retro_device_id_analog idAnalog = (retro_device_id_analog)id;
                    switch (idAnalog)
                    {
                        case retro_device_id_analog.RETRO_DEVICE_ID_ANALOG_X:
                        {
                            switch (index)
                            {
                                case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_LEFT:
                                {
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_X_PLUS] = descText;
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_X_MINUS] = descText;
                                }
                                break;
                                case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                                {
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_X_PLUS] = descText;
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_X_MINUS] = descText;
                                }
                                break;
                            }
                        }
                        break;
                        case retro_device_id_analog.RETRO_DEVICE_ID_ANALOG_Y:
                        {
                            switch (index)
                            {
                                case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_LEFT:
                                {
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_PLUS] = descText;
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_MINUS] = descText;
                                }
                                break;
                                case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                                {
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_Y_PLUS] = descText;
                                    Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_Y_MINUS] = descText;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                else
                {
                    Core.ButtonDescriptions[port, id] = descText;
                }
            }

            Core.HasInputDescriptors = true;

            return true;
        }

        // FIXME(Tom): not working
        private unsafe bool SetHwRender(void* data)
        {
            retro_hw_render_callback* inHwRenderCallback = (retro_hw_render_callback*)data;
            if (inHwRenderCallback->context_type != retro_hw_context_type.RETRO_HW_CONTEXT_OPENGL || !_glSupport)
            {
                ForceQuit = true;
                return false;
            }

            inHwRenderCallback->get_current_framebuffer = Marshal.GetFunctionPointerForDelegate(_videoDriverGetCurrentFrameBufferCallback);
            inHwRenderCallback->get_proc_address        = Marshal.GetFunctionPointerForDelegate(_videoDriverGetProcAddressCallback);
            _hwRenderCallback = *inHwRenderCallback;

            return true;
        }

        private unsafe bool SetVariables(void* data)
        {
            try
            {
                retro_variable* inVariable = (retro_variable*)data;

                Core.CoreOptions = _coreOptionsList.Cores.Find(x => x.CoreName.Equals(Core.CoreName, StringComparison.OrdinalIgnoreCase));
                if (Core.CoreOptions == null)
                {
                    Core.CoreOptions = new CoreOptions { CoreName = Core.CoreName };
                    _coreOptionsList.Cores.Add(Core.CoreOptions);
                }

                while (inVariable->key != null)
                {
                    string key        = CharsToString(inVariable->key);
                    string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
                    if (coreOption == null)
                    {
                        string inValue                = CharsToString(inVariable->value);
                        string[] descriptionAndValues = inValue.Split(';');
                        string[] possibleValues       = descriptionAndValues[1].Trim().Split('|');
                        string defaultValue           = possibleValues[0];
                        string value                  = defaultValue;
                        coreOption                    = $"{key};{value};{string.Join("|", possibleValues)};";
                        Core.CoreOptions.Options.Add(coreOption);
                    }
                    ++inVariable;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            SaveCoreOptionsFile();

            return true;
        }

        private unsafe bool SetSupportNoGame(void* data)
        {
            Core.SupportNoGame = *(bool*)data;
            return true;
        }

        private unsafe bool SetFrameTimeCallback(void* data)
        {
            retro_frame_time_callback* inFrameTimeCallback = (retro_frame_time_callback*)data;
            Core.SetFrameTimeCallback(inFrameTimeCallback->callback, inFrameTimeCallback->reference);
            return true;
        }

        private unsafe bool SetSystemAvInfo(void* data)
        {
            Game.SystemAVInfo = *(retro_system_av_info*)data;
            return true;
        }

        private unsafe bool SetControllerInfo(void* data)
        {
            retro_controller_info* inControllerInfo = (retro_controller_info*)data;

            int numPorts;
            for (numPorts = 0; inControllerInfo[numPorts].types != null; ++numPorts)
            {
                Log.Info($"# Controller port: {numPorts + 1}", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                for (int j = 0; j < inControllerInfo[numPorts].num_types; ++j)
                {
                    string desc = CharsToString(inControllerInfo[numPorts].types[j].desc);
                    uint id     = inControllerInfo[numPorts].types[j].id;
                    Log.Info($"    {desc} (ID: {id})", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                }
            }

            Core.ControllerPorts = new retro_controller_info[numPorts];
            for (int j = 0; j < numPorts; ++j)
            {
                Core.ControllerPorts[j] = inControllerInfo[j];
            }

            return true;
        }

        private unsafe bool SetGeometry(void* data)
        {
            if (Game.Running)
            {
                retro_game_geometry* inGeometry = (retro_game_geometry*)data;

                if (Game.SystemAVInfo.geometry.base_width != inGeometry->base_width
                 || Game.SystemAVInfo.geometry.base_height != inGeometry->base_height
                 || Game.SystemAVInfo.geometry.aspect_ratio != inGeometry->aspect_ratio)
                {
                    Game.SystemAVInfo.geometry = *inGeometry;
                    // TODO: Set video aspect ratio
                }
            }

            return true;
        }

        private unsafe bool SetCoreOptionsIntl(void* data)
        {
            retro_core_options_intl inOptionsIntl = Marshal.PtrToStructure<retro_core_options_intl>((IntPtr)data);

            Core.CoreOptions = _coreOptionsList.Cores.Find(x => x.CoreName.Equals(Core.CoreName, StringComparison.OrdinalIgnoreCase));
            if (Core.CoreOptions == null)
            {
                Core.CoreOptions = new CoreOptions { CoreName = Core.CoreName };
                _coreOptionsList.Cores.Add(Core.CoreOptions);
            }

            for (int i = 0; i < RETRO_NUM_CORE_OPTION_VALUES_MAX; i++)
            {
                IntPtr ins = new IntPtr(inOptionsIntl.us.ToInt64() + i * Marshal.SizeOf<retro_core_option_definition>());
                retro_core_option_definition defs = Marshal.PtrToStructure<retro_core_option_definition>(ins);
                if (defs.key == null)
                {
                    break;
                }

                string key = CharsToString(defs.key);

                string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
                if (coreOption == null)
                {
                    string defaultValue = CharsToString(defs.default_value);

                    List<string> possibleValues = new List<string>();
                    for (int j = 0; j < defs.values.Length; j++)
                    {
                        retro_core_option_value val = defs.values[j];
                        if (val.value != null)
                        {
                            possibleValues.Add(CharsToString(val.value));
                        }
                    }

                    string value = string.Empty;
                    if (!string.IsNullOrEmpty(defaultValue))
                    {
                        value = defaultValue;
                    }
                    else if (possibleValues.Count > 0)
                    {
                        value = possibleValues[0];
                    }

                    coreOption = $"{key};{value};{string.Join("|", possibleValues)}";

                    Core.CoreOptions.Options.Add(coreOption);
                }
            }

            SaveCoreOptionsFile();

            return true;
        }

        private bool Shutdown()
        {
            ForceQuit = true;
            return true;
        }
        #endregion
    }
}
