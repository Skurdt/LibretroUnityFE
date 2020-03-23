using SK.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        private const uint SUBSYSTEM_MAX_SUBSYSTEMS = 20;
        private const uint SUBSYSTEM_MAX_SUBSYSTEM_ROMS = 10;

        private readonly RetroSubsystemInfo[] subsystem_data = new RetroSubsystemInfo[SUBSYSTEM_MAX_SUBSYSTEMS];
        private readonly unsafe RetroSubsystemRomInfo*[] subsystem_data_roms = new RetroSubsystemRomInfo*[SUBSYSTEM_MAX_SUBSYSTEMS];
        private uint subsystem_current_count;

        [return: MarshalAs(UnmanagedType.U1)]
        private unsafe bool RetroEnvironmentCallback(RetroEnvironment cmd, void* data)
        {
            switch (cmd)
            {
                case RetroEnvironment.SET_ROTATION:
                {
                    //TODO: Rotate screen (counter-clockwise)
                    //Values: 0, 1,  2,   3
                    //Result: 0, 90, 180, 270 degrees
                    uint* inRotation = (uint*)data;
                    _core.rotation = *inRotation;
                    Log.Info($"in_Rotation: {_core.rotation}", "SET_ROTATION");
                    return false; // TODO: Remove when implemented!
                }
                //break;
                case RetroEnvironment.GET_OVERSCAN:
                {
                    //TODO: Retrieve value from core/rom
                    bool* outOverscan = (bool*)data;
                    *outOverscan = true;
                    Log.Info($"out_Overscan: {*outOverscan}", "GET_OVERSCAN");
                }
                break;
                case RetroEnvironment.GET_CAN_DUPE:
                {
                    //NOTE: What is frame duping and do we support that? No clue :p
                    bool* outCanDupe = (bool*)data;
                    *outCanDupe = true;
                }
                break;
                case RetroEnvironment.SET_MESSAGE:
                {
                    RetroMessage* inMessage = (RetroMessage*)data;
                    string msgString = Marshal.PtrToStringAnsi((IntPtr)inMessage->msg);
                    Log.Info($"in_Message: {msgString}", "SET_MESSAGE");
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.SHUTDOWN:
                //    break;
                case RetroEnvironment.SET_PERFORMANCE_LEVEL:
                {
                    int* inPerformanceLevel = (int*)data;
                    Log.Info($"in_PerformanceLevel: {*inPerformanceLevel}", "SET_PERFORMANCE_LEVEL");
                    return false; //TODO: Remove when implemented!
                }
                //break;
                case RetroEnvironment.GET_SYSTEM_DIRECTORY:
                {
                    char** outSystemDirectory = (char**)data;
                    *outSystemDirectory = StringToChar(_systemDirectory);
                    Log.Info($"out_SystemDirectory: {_systemDirectory}", "GET_SYSTEM_DIRECTORY");
                }
                break;
                case RetroEnvironment.SET_PIXEL_FORMAT:
                {
                    RetroPixelFormat* inPixelFormat = (RetroPixelFormat*)data;
                    switch (*inPixelFormat)
                    {
                        case RetroPixelFormat.RETRO_PIXEL_FORMAT_0RGB1555:
                        case RetroPixelFormat.RETRO_PIXEL_FORMAT_XRGB8888:
                        case RetroPixelFormat.RETRO_PIXEL_FORMAT_RGB565:
                        {
                            _pixelFormat = *inPixelFormat;
                            Log.Info($"in_PixelFormat: {_pixelFormat}", "SET_PIXEL_FORMAT");
                        }
                        break;
                        default:
                            return false;
                    }
                }
                break;
                case RetroEnvironment.SET_INPUT_DESCRIPTORS:
                {
                    RetroInputDescriptor* inInputDescriptors = (RetroInputDescriptor*)data;
                    while (inInputDescriptors->desc != null)
                    {
                        uint port = inInputDescriptors->port;
                        uint device = inInputDescriptors->device;
                        uint index = inInputDescriptors->index;
                        uint id = inInputDescriptors->id;
                        string descText = Marshal.PtrToStringAnsi((IntPtr)inInputDescriptors->desc);
                        Log.Info($"### Port: {port} Device: {(RetroDevice)device} Index: {index} Id: {id} Desc: {descText}", "SET_INPUT_DESCRIPTORS");
                        inInputDescriptors++;
                    }
                    return false; //TODO: Remove when implemented!
                }
                //break;
                case RetroEnvironment.SET_KEYBOARD_CALLBACK:
                {
                    Log.Info("SET_KEYBOARD_CALLBACK");
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.SET_DISK_CONTROL_INTERFACE:
                //    return false;
                //case RetroEnvironment.SET_HW_RENDER:
                //    break;
                case RetroEnvironment.GET_VARIABLE:
                {
                    RetroVariable* outVariable = (RetroVariable*)data;
                    string k = Marshal.PtrToStringAnsi((IntPtr)outVariable->key);
                    try
                    {
                        string v = _environmentSettings[k];
                        outVariable->value = StringToChar(v);
                    }
                    catch (KeyNotFoundException e)
                    {
                        Log.Exception(e.Message.Replace("given key", $"key '{k}'"));
                        return false;
                    }
                }
                break;
                case RetroEnvironment.SET_VARIABLES:
                {
                    RetroVariable* inVariable = (RetroVariable*)data;
                    while (inVariable->key != null)
                    {
                        string k = Marshal.PtrToStringAnsi((IntPtr)inVariable->key);
                        string v = Marshal.PtrToStringAnsi((IntPtr)inVariable->value);
                        string options = v.Split(';')[1];
                        string default_option = options.Split('|')[0].Trim();
                        _environmentSettings.Add(k, default_option);
                        inVariable++;
                    }
                }
                break;
                case RetroEnvironment.GET_VARIABLE_UPDATE:
                {
                    bool* outVariableUpdate = (bool*)data;
                    *outVariableUpdate = false;
                }
                break;
                //case RetroEnvironment.SET_SUPPORT_NO_GAME:
                //    break;
                //case RetroEnvironment.GET_LIBRETRO_PATH:
                //    break;
                //case RetroEnvironment.SET_FRAME_TIME_CALLBACK:
                //    break;
                //case RetroEnvironment.SET_AUDIO_CALLBACK:
                //    break;
                //case RetroEnvironment.GET_RUMBLE_INTERFACE:
                //    break;
                case RetroEnvironment.GET_INPUT_DEVICE_CAPABILITIES:
                {
                    Log.Info("GET_INPUT_DEVICE_CAPABILITIES");
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.GET_SENSOR_INTERFACE:
                //    break;
                //case RetroEnvironment.GET_CAMERA_INTERFACE:
                //    break;
                case RetroEnvironment.GET_LOG_INTERFACE:
                {
                    RetroLogCallback* outLogInterface = (RetroLogCallback*)data;
                    outLogInterface->log = Marshal.GetFunctionPointerForDelegate(_logPrintfCallback);
                }
                break;
                //case RetroEnvironment.GET_PERF_INTERFACE:
                //    break;
                //case RetroEnvironment.GET_LOCATION_INTERFACE:
                //    break;
                case RetroEnvironment.GET_CORE_ASSETS_DIRECTORY:
                {
                    char** outAssetDir = (char**)data;
                    *outAssetDir = StringToChar(_systemDirectory);
                }
                break;
                case RetroEnvironment.GET_SAVE_DIRECTORY:
                {
                    char** outSaveDir = (char**)data;
                    *outSaveDir = StringToChar(_systemDirectory);
                    //Debug.Log($"<color=yellow>Save directory:</color> {_systemDirectory}");
                }
                break;
                case RetroEnvironment.SET_SYSTEM_AV_INFO:
                {
                    RetroSystemAVInfo* inSystemAVnfo = (RetroSystemAVInfo*)data;
                    _systemAVInfo = *inSystemAVnfo;
                }
                break;
                //case RetroEnvironment.SET_PROC_ADDRESS_CALLBACK:
                //    break;
                case RetroEnvironment.SET_SUBSYSTEM_INFO:
                {
                    //RetroSubsystemInfo* subsytemInfo = (RetroSubsystemInfo*)data;
                    ////Debug.Log("<color=yellow>Subsystem Info:</color>");
                    ////Debug.Log($"<color=yellow>Description:</color> {Marshal.PtrToStringAnsi((IntPtr)subsytemInfo->desc)}");
                    ////Debug.Log($"<color=yellow>Ident:</color> {Marshal.PtrToStringAnsi((IntPtr)subsytemInfo->ident)}");
                    //_game_type = subsytemInfo->id;
                    //_num_info = subsytemInfo->num_roms;
                    //while (subsytemInfo->roms != null)
                    //{
                    //    RetroSubsystemRomInfo* romInfo = subsytemInfo->roms;
                    //    //Debug.Log("<color=orange>Rom Info:</color>");
                    //    //Debug.Log($"<color=orange>Description:</color> {Marshal.PtrToStringAnsi((IntPtr)romInfo->desc)}");
                    //    //Debug.Log($"<color=orange>Extensions:</color> {Marshal.PtrToStringAnsi((IntPtr)romInfo->valid_extensions)}");
                    //    subsytemInfo++;
                    //}

                    RetroSubsystemInfo* inSubsytemInfo = (RetroSubsystemInfo*)data;
                    // settings_t* settings = configuration_settings;
                    // unsigned log_level = settings->uints.frontend_log_level;

                    subsystem_current_count = 0;

                    uint size = 0;
                    Log.Info("SET_SUBSYSTEM_INFO", "Environment");
                    {
                        uint i = 0;
                        while (inSubsytemInfo[i].ident != null)
                        {
                            string subsystemDesc = Marshal.PtrToStringAnsi((IntPtr)inSubsytemInfo[i].desc);
                            string subsystemIdent = Marshal.PtrToStringAnsi((IntPtr)inSubsytemInfo[i].ident);
                            uint subsystemId = inSubsytemInfo[i].id;

                            Log.Info($"Subsystem ID: {i}");
                            Log.Info($"Special game type: {subsystemDesc}\n  Ident: {subsystemIdent}\n  ID: {subsystemId}\n  Content:");
                            for (uint j = 0; j < inSubsytemInfo[i].num_roms; j++)
                            {
                                string romDesc = Marshal.PtrToStringAnsi((IntPtr)inSubsytemInfo[i].roms[j].desc);
                                string required = inSubsytemInfo[i].roms[j].required ? "required" : "optional";
                                Log.Info($"    {romDesc} ({required})");
                            }
                            i++;
                        }

                        //if (log_level == RETRO_LOG_DEBUG)
                        Log.Info($"Subsystems: {i}");
                        size = i;
                    }
                    //if (log_level == RETRO_LOG_DEBUG)
                    if (size > SUBSYSTEM_MAX_SUBSYSTEMS)
                    {
                        Log.Warning($"Subsystems exceed subsystem max, clamping to {SUBSYSTEM_MAX_SUBSYSTEMS}");
                    }

                    if (_core != null)
                    {
                        for (uint i = 0; i < size && i < SUBSYSTEM_MAX_SUBSYSTEMS; i++)
                        {
                            ref RetroSubsystemInfo subdata = ref subsystem_data[i];

                            subdata.desc = inSubsytemInfo[i].desc;
                            subdata.ident = inSubsytemInfo[i].ident;
                            subdata.id = inSubsytemInfo[i].id;
                            subdata.num_roms = inSubsytemInfo[i].num_roms;

                            //if (log_level == RETRO_LOG_DEBUG)
                            if (subdata.num_roms > SUBSYSTEM_MAX_SUBSYSTEM_ROMS)
                            {
                                Log.Warning($"Subsystems exceed subsystem max roms, clamping to {SUBSYSTEM_MAX_SUBSYSTEM_ROMS}");
                            }

                            for (uint j = 0; j < subdata.num_roms && j < SUBSYSTEM_MAX_SUBSYSTEM_ROMS; j++)
                            {
                                while (subdata.roms != null)
                                {
                                    RetroSubsystemRomInfo* romInfo = subdata.roms;
                                    romInfo->desc = inSubsytemInfo[i].roms[j].desc;
                                    romInfo->valid_extensions = inSubsytemInfo[i].roms[j].valid_extensions;
                                    romInfo->required = inSubsytemInfo[i].roms[j].required;
                                    romInfo->block_extract = inSubsytemInfo[i].roms[j].block_extract;
                                    romInfo->need_fullpath = inSubsytemInfo[i].roms[j].need_fullpath;
                                    subdata.roms++;
                                }
                            }

                            subdata.roms = subsystem_data_roms[i];
                        }

                        subsystem_current_count = (size <= SUBSYSTEM_MAX_SUBSYSTEMS) ? size : SUBSYSTEM_MAX_SUBSYSTEMS;
                    }
                    return false; //TODO: Remove when implemented!
                }
                //break;
                case RetroEnvironment.SET_CONTROLLER_INFO:
                {
                    RetroControllerInfo* inControllerInfo = (RetroControllerInfo*)data;
                    while (inControllerInfo->types != null)
                    {
                        RetroControllerDescription* description = inControllerInfo->types;
                        string descText = Marshal.PtrToStringAnsi((IntPtr)description->desc);
                        uint id = description->id;
                        Log.Info($"#### Controller {id} description: {descText}");
                        inControllerInfo++;
                    }
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.SET_MEMORY_MAPS:
                //    break;
                case RetroEnvironment.SET_GEOMETRY:
                {
                    RetroGameGeometry* inGeometry = (RetroGameGeometry*)data;
                    if ((_systemAVInfo.geometry.base_width != inGeometry->base_width)
                        || (_systemAVInfo.geometry.base_height != inGeometry->base_height)
                        || (_systemAVInfo.geometry.aspect_ratio != inGeometry->aspect_ratio))
                    {
                        _systemAVInfo.geometry.base_width = inGeometry->base_width;
                        _systemAVInfo.geometry.base_height = inGeometry->base_height;
                        _systemAVInfo.geometry.aspect_ratio = inGeometry->aspect_ratio;

                        // TODO: Set video aspect ratio
                    }
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.GET_USERNAME:
                //    break;
                //case RetroEnvironment.GET_LANGUAGE:
                //    break;
                //case RetroEnvironment.GET_CURRENT_SOFTWARE_FRAMEBUFFER:
                //    break;
                //case RetroEnvironment.GET_HW_RENDER_INTERFACE:
                //    break;
                //case RetroEnvironment.SET_SUPPORT_ACHIEVEMENTS:
                //    break;
                //case RetroEnvironment.SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE:
                //    break;
                //case RetroEnvironment.SET_SERIALIZATION_QUIRKS:
                //{
                //ulong* quirk = (ulong*)data;
                //}
                //break;
                //case RetroEnvironment.SET_HW_SHARED_CONTEXT:
                //    break;
                case RetroEnvironment.GET_VFS_INTERFACE:
                    return false;
                case RetroEnvironment.GET_LED_INTERFACE:
                    return false;
                case RetroEnvironment.GET_AUDIO_VIDEO_ENABLE:
                {
                    int result = 0;
                    result |= 1; // if video enabled
                    result |= 2; // if audio enabled

                    int* outAudioVideoEnabled = (int*)data;
                    *outAudioVideoEnabled = result;
                }
                break;
                //case RetroEnvironment.GET_MIDI_INTERFACE:
                //    break;
                //case RetroEnvironment.GET_FASTFORWARDING:
                //    break;
                //case RetroEnvironment.GET_TARGET_REFRESH_RATE:
                //    break;
                case RetroEnvironment.GET_INPUT_BITMASKS:
                {
                }
                break;
                case RetroEnvironment.GET_CORE_OPTIONS_VERSION:
                {
                    // NOTE: Retroarch says: Current API version is 1
                    uint* outVersion = (uint*)data;
                    *outVersion = 1;
                }
                break;
                //case RetroEnvironment.SET_CORE_OPTIONS:
                //    break;
                case RetroEnvironment.SET_CORE_OPTIONS_INTL:
                {
                    // TODO: Something probably
                    return false; //TODO: Remove when implemented!
                }
                //break;
                //case RetroEnvironment.SET_CORE_OPTIONS_DISPLAY:
                //    break;
                //case RetroEnvironment.GET_PREFERRED_HW_RENDER:
                //    break;
                //case RetroEnvironment.GET_DISK_CONTROL_INTERFACE_VERSION:
                //    break;
                //case RetroEnvironment.SET_DISK_CONTROL_EXT_INTERFACE:
                //    break;
                default:
                {
                    Debug.Log($"<color=red>Not implemented:</color> {Enum.GetName(typeof(RetroEnvironment), cmd)}");
                    return false;
                }
            }

            return true;
        }
    }
}
