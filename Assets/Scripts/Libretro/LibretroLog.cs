using System;
using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private void RetroLogPrintf(retro_log_level log_level, string format, IntPtr args)
        {
            if (log_level > retro_log_level.RETRO_LOG_INFO)
            {
                Debug.Log($"{log_level}: {format}");
            }
        }
    }
}
