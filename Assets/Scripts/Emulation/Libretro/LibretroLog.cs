using System;
using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        private void RetroLogPrintf(RetroLogLevel log_level, string format, IntPtr args)
        {
            if (log_level > RetroLogLevel.RETRO_LOG_INFO)
            {
                Debug.Log($"{log_level.ToString()}: {format}");
            }
        }
    }
}
