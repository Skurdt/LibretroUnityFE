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

        public retro_system_av_info SystemAVInfo;
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
            if (_core.retro_load_game(ref gameInfo))
            {
                try
                {
                    SystemAVInfo = new retro_system_av_info();
                    _core.retro_get_system_av_info(ref SystemAVInfo);

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
