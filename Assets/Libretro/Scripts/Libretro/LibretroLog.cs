using SK.Utilities;
using System;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public void RetroLogPrintf(retro_log_level log_level, string format, IntPtr _/*args*/)
        {
            if (log_level > retro_log_level.RETRO_LOG_INFO)
            {
                Log.Info($"{log_level}: {format}", "Libretro.Wrapper.RetroLogPrintf");
            }
        }
    }
}
