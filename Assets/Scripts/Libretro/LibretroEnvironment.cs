using SK.Utilities;
using System;
using System.Runtime.InteropServices;
using static SK.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        //private const uint SUBSYSTEM_MAX_SUBSYSTEMS = 20;
        //private const uint SUBSYSTEM_MAX_SUBSYSTEM_ROMS = 10;

        //private readonly RetroSubsystemInfo[] subsystem_data = new RetroSubsystemInfo[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private readonly unsafe RetroSubsystemRomInfo*[] subsystem_data_roms = new RetroSubsystemRomInfo*[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private uint subsystem_current_count;

        [return: MarshalAs(UnmanagedType.U1)]
        private unsafe bool RetroEnvironmentCallback(retro_environment cmd, void* data)
        {
            switch (cmd)
            {
                case retro_environment.RETRO_ENVIRONMENT_SET_ROTATION:
                {
                    uint* inRotation = (uint*)data;
                    Log.Warning($"in_Rotation: {*inRotation}", "RETRO_ENVIRONMENT_SET_ROTATION");
                    if (Core != null)
                    {
                        Core.Rotation = (int)*inRotation;
                    }
                    return false; // TODO: Rotate screen (counter-clockwise) Values: 0, 1, 2, 3 Result: 0, 90, 180, 270 degrees
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
                    //NOTE: What is frame duping and do we support that? No clue :p
                    bool* outCanDupe = (bool*)data;
                    *outCanDupe      = true;
                    Log.Warning($"out_CanDupe: {*outCanDupe}", "RETRO_ENVIRONMENT_GET_CAN_DUPE");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_MESSAGE:
                {
                    retro_message* inMessage = (retro_message*)data;
                    string msgString         = CharsToString(inMessage->msg);
                    Log.Warning($"in_Message: {msgString}", "RETRO_ENVIRONMENT_SET_MESSAGE");
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SHUTDOWN:
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
                    string systemDirectory    = FileSystem.GetAbsolutePath($"{SystemDirectory}/{Core.CoreName}");
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
                    while (inInputDescriptors->desc != null)
                    {
                        uint port       = inInputDescriptors->port;
                        uint device     = inInputDescriptors->device;
                        uint index      = inInputDescriptors->index;
                        uint id         = inInputDescriptors->id;
                        string descText = CharsToString(inInputDescriptors->desc);
                        Log.Warning($"### Port: {port} Device: {(retro_device)device} Index: {index} Id: {id} Desc: {descText}", "RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS");
                        inInputDescriptors++;
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK:
                {
                    Log.Warning("RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK");
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE:
                //    return false;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_HW_RENDER:
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
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_LIBRETRO_PATH:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES:
                {
                    Log.Warning("RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES");
                    //ulong* outBitmask = (ulong*)data;
                    //*outBitmask       = (1 << (int)RetroDevice.RETRO_DEVICE_JOYPAD) | (1 << (int)RetroDevice.RETRO_DEVICE_ANALOG) | (1 << (int)RetroDevice.RETRO_DEVICE_KEYBOARD);
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LOG_INTERFACE:
                {
                    retro_log_callback* outLogInterface = (retro_log_callback*)data;
                    outLogInterface->log                = Marshal.GetFunctionPointerForDelegate(_logPrintfCallback);
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_PERF_INTERFACE:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE:
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
                    string saveDirectory    = FileSystem.GetAbsolutePath($"{SystemDirectory}/{Core.CoreName}");
                    *outSaveDirectory       = StringToChars(saveDirectory);
                    Log.Info($"out_SaveDirectory: {saveDirectory}", "RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY");
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
                //{
                //    RetroSystemAVInfo* inSystemAVnfo = (RetroSystemAVInfo*)data;
                //    _systemAVInfo = *inSystemAVnfo;
                //}
                //break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO:
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
                    int i;
                    retro_controller_info* inControllerInfo = (retro_controller_info*)data;
                    for (i = 0; inControllerInfo[i].types != null; ++i)
                    {
                        Log.Info($"# Controller port: {i + 1}", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        for (int j = 0; j < inControllerInfo[i].num_types; ++j)
                        {
                            string desc = CharsToString(inControllerInfo[i].types[j].desc);
                            uint id     = inControllerInfo[i].types[j].id;
                            Log.Info($"    {desc} (ID: {id})", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        }
                    }

                    if (Core != null)
                    {
                        int numPorts = i;
                        Core.ControllerPorts = new retro_controller_info[numPorts];
                        for (int j = 0; j < numPorts; ++j)
                        {
                            Core.ControllerPorts[j] = inControllerInfo[j];
                        }
                    }
                    else
                    {
                        Log.Error($"Core is null.", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                    }
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_MEMORY_MAPS:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_GEOMETRY:
                {
                    retro_game_geometry* inGeometry = (retro_game_geometry*)data;
                    if ((_systemAVInfo.geometry.base_width != inGeometry->base_width)
                        || (_systemAVInfo.geometry.base_height != inGeometry->base_height)
                        || (_systemAVInfo.geometry.aspect_ratio != inGeometry->aspect_ratio))
                    {
                        _systemAVInfo.geometry.base_width   = inGeometry->base_width;
                        _systemAVInfo.geometry.base_height  = inGeometry->base_height;
                        _systemAVInfo.geometry.aspect_ratio = inGeometry->aspect_ratio;

                        // TODO: Set video aspect ratio
                    }
                }
                break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_USERNAME:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LANGUAGE:
                    return false;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS:
                    return false;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS:
                //{
                //ulong* quirk = (ulong*)data;
                //}
                //break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT:
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
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_MIDI_INTERFACE:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_FASTFORWARDING:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE:
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
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS:
                //    break;
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL:
                    return false;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_PREFERRED_HW_RENDER:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_DISK_CONTROL_INTERFACE_VERSION:
                //    break;
                //case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE:
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
