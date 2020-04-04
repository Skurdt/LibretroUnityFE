using SK.Utilities;
using System;
using System.Runtime.InteropServices;
using static SK.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public const int MAX_USERS = 16;

        private const int FIRST_CUSTOM_BIND      = 16;
        private const int FIRST_LIGHTGUN_BIND    = (int)CustomBinds.ANALOG_BIND_LIST_END;
        private const int FIRST_MISC_CUSTOM_BIND = (int)CustomBinds.LIGHTGUN_BIND_LIST_END;
        public const int FIRST_META_KEY          = (int)CustomBinds.CUSTOM_BIND_LIST_END;

        /* RetroArch specific bind IDs. */
        private enum CustomBinds
        {
            // Custom binds that extend the scope of RETRO_DEVICE_JOYPAD for RetroArch specifically.
            // Analogs (RETRO_DEVICE_ANALOG)
            ANALOG_LEFT_X_PLUS = FIRST_CUSTOM_BIND,
            ANALOG_LEFT_X_MINUS,
            ANALOG_LEFT_Y_PLUS,
            ANALOG_LEFT_Y_MINUS,
            ANALOG_RIGHT_X_PLUS,
            ANALOG_RIGHT_X_MINUS,
            ANALOG_RIGHT_Y_PLUS,
            ANALOG_RIGHT_Y_MINUS,
            ANALOG_BIND_LIST_END,

            // Lightgun
            LIGHTGUN_TRIGGER = FIRST_LIGHTGUN_BIND,
            LIGHTGUN_RELOAD,
            LIGHTGUN_AUX_A,
            LIGHTGUN_AUX_B,
            LIGHTGUN_AUX_C,
            LIGHTGUN_START,
            LIGHTGUN_SELECT,
            LIGHTGUN_DPAD_UP,
            LIGHTGUN_DPAD_DOWN,
            LIGHTGUN_DPAD_LEFT,
            LIGHTGUN_DPAD_RIGHT,
            LIGHTGUN_BIND_LIST_END,

            // Turbo
            TURBO_ENABLE = FIRST_MISC_CUSTOM_BIND,

            CUSTOM_BIND_LIST_END,

            // Command binds. Not related to game input, only usable for port 0.
            FAST_FORWARD_KEY = FIRST_META_KEY,
            FAST_FORWARD_HOLD_KEY,
            SLOWMOTION_KEY,
            SLOWMOTION_HOLD_KEY,
            LOAD_STATE_KEY,
            SAVE_STATE_KEY,
            FULLSCREEN_TOGGLE_KEY,
            QUIT_KEY,
            STATE_SLOT_PLUS,
            STATE_SLOT_MINUS,
            REWIND,
            BSV_RECORD_TOGGLE,
            PAUSE_TOGGLE,
            FRAMEADVANCE,
            RESET,
            SHADER_NEXT,
            SHADER_PREV,
            CHEAT_INDEX_PLUS,
            CHEAT_INDEX_MINUS,
            CHEAT_TOGGLE,
            SCREENSHOT,
            MUTE,
            OSK,
            FPS_TOGGLE,
            SEND_DEBUG_INFO,
            NETPLAY_HOST_TOGGLE,
            NETPLAY_GAME_WATCH,
            ENABLE_HOTKEY,
            VOLUME_UP,
            VOLUME_DOWN,
            OVERLAY_NEXT,
            DISK_EJECT_TOGGLE,
            DISK_NEXT,
            DISK_PREV,
            GRAB_MOUSE_TOGGLE,
            GAME_FOCUS_TOGGLE,
            UI_COMPANION_TOGGLE,

            MENU_TOGGLE,

            RECORDING_TOGGLE,
            STREAMING_TOGGLE,

            AI_SERVICE,

            BIND_LIST_END,
            BIND_LIST_END_NULL
        };

        //private const uint SUBSYSTEM_MAX_SUBSYSTEMS = 20;
        //private const uint SUBSYSTEM_MAX_SUBSYSTEM_ROMS = 10;

        //private readonly RetroSubsystemInfo[] subsystem_data = new RetroSubsystemInfo[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private readonly unsafe RetroSubsystemRomInfo*[] subsystem_data_roms = new RetroSubsystemRomInfo*[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private uint subsystem_current_count;

        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe bool RetroEnvironmentCallback(retro_environment cmd, void* data)
        {
            switch (cmd)
            {
                case retro_environment.RETRO_ENVIRONMENT_SET_ROTATION:
                {
                    // TODO: Rotate screen (counter-clockwise)
                    // Values: 0,  1,   2,   3
                    // Result: 0, 90, 180, 270 degrees
                    uint* inRotation = (uint*)data;
                    Core.Rotation = (int)*inRotation;
                    Log.Warning($"in_Rotation: {*inRotation}", "RETRO_ENVIRONMENT_SET_ROTATION");
                    return false;
                }
                //break;
                case retro_environment.RETRO_ENVIRONMENT_GET_OVERSCAN:
                {
                    // TODO: Figure out the value...
                    bool* outOverscan = (bool*)data;
                    *outOverscan      = true;
                    Log.Warning($"out_Overscan: {*outOverscan}", "RETRO_ENVIRONMENT_GET_OVERSCAN");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_CAN_DUPE:
                {
                    bool* outCanDupe = (bool*)data;
                    *outCanDupe      = true;
                    Log.Info($"out_CanDupe: {*outCanDupe}", "RETRO_ENVIRONMENT_GET_CAN_DUPE");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_MESSAGE:
                {
                    retro_message* inMessage = (retro_message*)data;
                    string msgString         = CharsToString(inMessage->msg);
                    Log.Warning($"in_Message: {msgString}", "RETRO_ENVIRONMENT_SET_MESSAGE");
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SHUTDOWN:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL:
                {
                    int* inPerformanceLevel = (int*)data;
                    Core.PerformanceLevel   = *inPerformanceLevel;
                    Log.Info($"in_PerformanceLevel: {*inPerformanceLevel}", "RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
                {
                    char** outSystemDirectory = (char**)data;
                    string systemDirectory    = FileSystem.GetAbsolutePath(SystemDirectory);
                    *outSystemDirectory       = StringToChars(systemDirectory);
                    Log.Info($"out_SystemDirectory: {systemDirectory}", "RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
                {
                    retro_pixel_format* inPixelFormat = (retro_pixel_format*)data;
                    switch (*inPixelFormat)
                    {
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                        {
                            _pixelFormat = *inPixelFormat;
                            Log.Info($"in_PixelFormat: {_pixelFormat}", "RETRO_ENVIRONMENT_SET_PIXEL_FORMAT");
                        }
                        break;
                        default:
                            return false;
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS:
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

                        string descText                 = CharsToString(inInputDescriptors->desc);
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
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_PLUS]  = descText;
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_MINUS] = descText;
                                        }
                                        break;
                                        case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                                        {
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_Y_PLUS]  = descText;
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
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK:
                {
                    Log.Warning("RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK");
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE:
                //    return false;
                //case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE:
                {
                    retro_variable* outVariable = (retro_variable*)data;

                    string key = CharsToString(outVariable->key);
                    if (Core.CoreOptions != null)
                    {
                        CoreOption coreOption = Core.CoreOptions.Options.Find(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (coreOption != null && coreOption.Value != null)
                        {
                            outVariable->value = StringToChars(coreOption.Value);
                        }
                        else
                        {
                            Log.Warning($"Core option {key} not found!");
                            return false;
                        }
                    }
                    else
                    {
                        Log.Warning($"Core didn't set its options for key '{key}'.");
                        return false;
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_VARIABLES:
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
                        string key = CharsToString(inVariable->key);

                        CoreOption coreOption = Core.CoreOptions.Options.Find(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (coreOption == null)
                        {
                            string[] descriptionAndValues = CharsToString(inVariable->value).Split(';');
                            string description            = descriptionAndValues[0].Trim();
                            string[] possibleValues       = descriptionAndValues[1].Trim().Split('|');
                            string defaultValue           = possibleValues[0];
                            string value                  = defaultValue;

                            coreOption = new CoreOption
                            {
                                Description    = description,
                                Value          = value,
                                Key            = key,
                                PossibleValues = possibleValues,
                                DefaultValue   = defaultValue
                            };

                            Core.CoreOptions.Options.Add(coreOption);
                        }

                        inVariable++;
                    }

                    _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE:
                {
                    bool* outVariableUpdate = (bool*)data;
                    *outVariableUpdate      = false;
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_LIBRETRO_PATH:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES:
                {
                    Log.Warning("RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES");
                    //ulong* outBitmask = (ulong*)data;
                    //*outBitmask       = (1 << (int)RetroDevice.RETRO_DEVICE_JOYPAD) | (1 << (int)RetroDevice.RETRO_DEVICE_ANALOG) | (1 << (int)RetroDevice.RETRO_DEVICE_KEYBOARD);
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LOG_INTERFACE:
                {
                    retro_log_callback* outLogInterface = (retro_log_callback*)data;
                    outLogInterface->log                = Core.SetLogCallback();
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_PERF_INTERFACE:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY:
                {
                    char** outCoreAssetsDirectory = (char**)data;
                    string coreAssetsDirectory    = FileSystem.GetAbsolutePath($"{SystemDirectory}/{Core.CoreName}");
                    *outCoreAssetsDirectory       = StringToChars(coreAssetsDirectory);
                    Log.Info($"out_CoreAssetsDirectory: {coreAssetsDirectory}", "RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
                {
                    char** outSaveDirectory = (char**)data;
                    string saveDirectory    = FileSystem.GetAbsolutePath($"{SavesDirectory}");
                    *outSaveDirectory       = StringToChars(saveDirectory);
                    Log.Info($"out_SaveDirectory: {saveDirectory}", "RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
                {
                    retro_system_av_info* inSystemAVnfo = (retro_system_av_info*)data;
                    Game.SetSystemAVInfo(*inSystemAVnfo);
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO:
                //{
                //    //RetroSubsystemInfo* subsytemInfo = (RetroSubsystemInfo*)data;
                //    ////Debug.Log("<color=yellow>Subsystem Info:</color>");
                //    ////Debug.Log($"<color=yellow>Description:</color> {Marshal.PtrToStringAnsisubsytemInfo->desc)}");
                //    ////Debug.Log($"<color=yellow>Ident:</color> {Marshal.PtrToStringAnsisubsytemInfo->ident)}");
                //    //_game_type = subsytemInfo->id;
                //    //_num_info = subsytemInfo->num_roms;
                //    //while (subsytemInfo->roms != null)
                //    //{
                //    //    RetroSubsystemRomInfo* romInfo = subsytemInfo->roms;
                //    //    //Debug.Log("<color=orange>Rom Info:</color>");
                //    //    //Debug.Log($"<color=orange>Description:</color> {Marshal.PtrToStringAnsiromInfo->desc)}");
                //    //    //Debug.Log($"<color=orange>Extensions:</color> {Marshal.PtrToStringAnsiromInfo->valid_extensions)}");
                //    //    subsytemInfo++;
                //    //}

                //    RetroSubsystemInfo* inSubsytemInfo = (RetroSubsystemInfo*)data;
                //    // settings_t* settings = configuration_settings;
                //    // unsigned log_level = settings->uints.frontend_log_level;

                //    subsystem_current_count = 0;

                //    uint size = 0;
                //    Log.Info("SET_SUBSYSTEM_INFO", "Environment");
                //    {
                //        uint i = 0;
                //        while (inSubsytemInfo[i].ident != null)
                //        {
                //            string subsystemDesc = Marshal.PtrToStringAnsiinSubsytemInfo[i].desc);
                //            string subsystemIdent = Marshal.PtrToStringAnsiinSubsytemInfo[i].ident);
                //            uint subsystemId = inSubsytemInfo[i].id;

                //            Log.Info($"Subsystem ID: {i}");
                //            Log.Info($"Special game type: {subsystemDesc}\n  Ident: {subsystemIdent}\n  ID: {subsystemId}\n  Content:");
                //            for (uint j = 0; j < inSubsytemInfo[i].num_roms; j++)
                //            {
                //                string romDesc = Marshal.PtrToStringAnsiinSubsytemInfo[i].roms[j].desc);
                //                string required = inSubsytemInfo[i].roms[j].required ? "required" : "optional";
                //                Log.Info($"    {romDesc} ({required})");
                //            }
                //            i++;
                //        }

                //        //if (log_level == RETRO_LOG_DEBUG)
                //        Log.Info($"Subsystems: {i}");
                //        size = i;
                //    }
                //    //if (log_level == RETRO_LOG_DEBUG)
                //    if (size > SUBSYSTEM_MAX_SUBSYSTEMS)
                //    {
                //        Log.Warning($"Subsystems exceed subsystem max, clamping to {SUBSYSTEM_MAX_SUBSYSTEMS}");
                //    }

                //    if (Core != null)
                //    {
                //        for (uint i = 0; i < size && i < SUBSYSTEM_MAX_SUBSYSTEMS; i++)
                //        {
                //            ref RetroSubsystemInfo subdata = ref subsystem_data[i];

                //            subdata.desc = inSubsytemInfo[i].desc;
                //            subdata.ident = inSubsytemInfo[i].ident;
                //            subdata.id = inSubsytemInfo[i].id;
                //            subdata.num_roms = inSubsytemInfo[i].num_roms;

                //            //if (log_level == RETRO_LOG_DEBUG)
                //            if (subdata.num_roms > SUBSYSTEM_MAX_SUBSYSTEM_ROMS)
                //            {
                //                Log.Warning($"Subsystems exceed subsystem max roms, clamping to {SUBSYSTEM_MAX_SUBSYSTEM_ROMS}");
                //            }

                //            for (uint j = 0; j < subdata.num_roms && j < SUBSYSTEM_MAX_SUBSYSTEM_ROMS; j++)
                //            {
                //                while (subdata.roms != null)
                //                {
                //                    RetroSubsystemRomInfo* romInfo = subdata.roms;
                //                    romInfo->desc = inSubsytemInfo[i].roms[j].desc;
                //                    romInfo->valid_extensions = inSubsytemInfo[i].roms[j].valid_extensions;
                //                    romInfo->required = inSubsytemInfo[i].roms[j].required;
                //                    romInfo->block_extract = inSubsytemInfo[i].roms[j].block_extract;
                //                    romInfo->need_fullpath = inSubsytemInfo[i].roms[j].need_fullpath;
                //                    subdata.roms++;
                //                }
                //            }

                //            subdata.roms = subsystem_data_roms[i];
                //        }

                //        subsystem_current_count = (size <= SUBSYSTEM_MAX_SUBSYSTEMS) ? size : SUBSYSTEM_MAX_SUBSYSTEMS;
                //    }
                //    return false; //TODO: Remove when implemented!
                //}
                //break;
                case retro_environment.RETRO_ENVIRONMENT_SET_CONTROLLER_INFO:
                {
                    retro_controller_info* inControllerInfo = (retro_controller_info*)data;

                    int numPorts;
                    for (numPorts = 0; inControllerInfo[numPorts].types != null; ++numPorts)
                    {
                        Log.Info($"# Controller port: {numPorts + 1}", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        for (int j = 0; j < inControllerInfo[numPorts].num_types; ++j)
                        {
                            string desc = CharsToString(inControllerInfo[numPorts].types[j].desc);
                            uint id = inControllerInfo[numPorts].types[j].id;
                            Log.Info($"    {desc} (ID: {id})", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        }
                    }

                    Core.ControllerPorts = new retro_controller_info[numPorts];
                    for (int j = 0; j < numPorts; ++j)
                    {
                        Core.ControllerPorts[j] = inControllerInfo[j];
                    }
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_MEMORY_MAPS:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_GEOMETRY:
                {
                    if (Game.Running)
                    {
                        retro_game_geometry* inGeometry = (retro_game_geometry*)data;
                        if (Game.BaseWidth != inGeometry->base_width || Game.BaseHeight != inGeometry->base_height || Game.AspectRatio != inGeometry->aspect_ratio)
                        {
                            Game.SetGeometry(*inGeometry);
                            // TODO: Set video aspect ratio
                        }
                    }
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_USERNAME:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LANGUAGE:
                    return false;
                //case retro_environment.RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS:
                    return false;
                //case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS:
                //{
                //ulong* quirk = (ulong*)data;
                //}
                //break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_VFS_INTERFACE:
                    return false;
                case retro_environment.RETRO_ENVIRONMENT_GET_LED_INTERFACE:
                    return false;
                case retro_environment.RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE:
                {
                    int result = 0;
                    result |= 1; // if video enabled
                    result |= 2; // if audio enabled

                    int* outAudioVideoEnabled = (int*)data;
                    *outAudioVideoEnabled = result;
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_MIDI_INTERFACE:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_FASTFORWARDING:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_BITMASKS:
                {
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION:
                {
                    uint* outVersion = (uint*)data;
                    *outVersion = RETRO_API_VERSION;
                }
                break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL:
                    return false;
                //case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_PREFERRED_HW_RENDER:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_GET_DISK_CONTROL_INTERFACE_VERSION:
                //    break;
                //case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE:
                //    break;
                default:
                {
                    Log.Error($"Not implemented: {Enum.GetName(typeof(retro_environment), cmd)}", "RetroEnvironmentCallback");
                    return false;
                }
            }

            return true;
        }
    }
}
