using SK.Libretro.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.Wrapper;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    public class LibretroGame
    {
        public string Name { get; private set; }

        public int BaseWidth { get; private set; }
        public int BaseHeight { get; private set; }
        public int MaxWidth { get; private set; }
        public int MaxHeight { get; private set; }
        public float AspectRatio { get; private set; }
        public float Fps { get; private set; }
        public int SampleRate { get; private set; }

        public retro_pixel_format PixelFormat;

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
                try
                {
                    retro_system_av_info avInfo = new retro_system_av_info();
                    core.retro_get_system_av_info(ref avInfo);
                    SetSystemAVInfo(avInfo);

                    Running = true;
                    result = true;
                }
                catch (Exception e)
                {
                    Log.Exception(e, "Libretro.LibretroGame.Start");
                }
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

        public void SetSystemAVInfo(retro_system_av_info systemAVInfo)
        {
            SetGeometry(systemAVInfo.geometry);
            SetTiming(systemAVInfo.timing);
        }

        public void SetGeometry(retro_game_geometry gameGeometry)
        {
            BaseWidth   = Convert.ToInt32(gameGeometry.base_width);
            BaseHeight  = Convert.ToInt32(gameGeometry.base_height);
            MaxWidth    = Convert.ToInt32(gameGeometry.max_width);
            MaxHeight   = Convert.ToInt32(gameGeometry.max_height);
            AspectRatio = gameGeometry.aspect_ratio;
        }

        private void SetTiming(retro_system_timing systemTiming)
        {
            Fps        = Convert.ToSingle(systemTiming.fps);
            SampleRate = Convert.ToInt32(systemTiming.sample_rate);
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
