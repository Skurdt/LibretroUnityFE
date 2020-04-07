using SK.Libretro.Utilities;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public long RetroPerfGetTimeUsec()
        {
            Log.Warning("RetroPerfGetTimeUsec");
            return 0;
        }

        public ulong RetroPerfGetCounter()
        {
            Log.Warning("RetroPerfGetCounter");
            return 0;
        }

        public ulong RetroGetCPUFeatures()
        {
            Log.Warning("RetroGetCPUFeatures");
            return 0;
        }

        public void RetroPerfLog()
        {
            Log.Warning("RetroPerfLog");
        }

        public void RetroPerfRegister(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfRegister");
        }

        public void RetroPerfStart(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfStart");
        }

        public void RetroPerfStop(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfStop");
        }
    }
}
