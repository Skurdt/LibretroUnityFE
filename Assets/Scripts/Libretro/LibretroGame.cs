using System;
using System.IO;
using System.Runtime.InteropServices;
using static SK.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public class LibretroGame
        {
            public string Name { get; private set; }

            public int BaseWidth;
            public int BaseHeight;
            public int MaxWidth;
            public int MaxHeight;
            public float AspectRatio;
            public float Fps;
            public int SampleRate;

            public bool Running { get; private set; }

            private LibretroCore _core;
            private IntPtr _internalData;

            public bool Start(LibretroCore core, string gamePath)
            {
                bool result = false;

                _core = core;
                Name = Path.GetFileNameWithoutExtension(gamePath);

                retro_game_info gameInfo = GetGameInfo(gamePath);
                if (core.retro_load_game(ref gameInfo))
                {
                    retro_system_av_info systemAVInfo = new retro_system_av_info();
                    core.retro_get_system_av_info(ref systemAVInfo);

                    BaseWidth   = (int)systemAVInfo.geometry.base_width;
                    BaseHeight  = (int)systemAVInfo.geometry.base_height;
                    MaxWidth    = (int)systemAVInfo.geometry.max_width;
                    MaxHeight   = (int)systemAVInfo.geometry.max_height;
                    AspectRatio = systemAVInfo.geometry.aspect_ratio;
                    Fps         = (float)systemAVInfo.timing.fps;
                    SampleRate  = (int)systemAVInfo.timing.sample_rate;

                    Running = true;
                    result = true;
                }

                return result;
            }

            public void Stop()
            {
                if (Running)
                {
                    _core?.retro_unload_game();
                    Running = false;
                }

                if (_internalData != null)
                {
                    Marshal.FreeHGlobal(_internalData);
                }
            }

            private unsafe retro_game_info GetGameInfo(string gamePath)
            {
                using (FileStream stream = new FileStream(gamePath, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    _ = stream.Read(data, 0, (int)stream.Length);
                    _internalData = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf<byte>());
                    Marshal.Copy(data, 0, _internalData, data.Length);
                    return new retro_game_info
                    {
                        path = StringToChars(gamePath),
                        size = Convert.ToUInt32(data.Length),
                        data = _internalData.ToPointer()
                    };
                }
            }
        }
    }
}
