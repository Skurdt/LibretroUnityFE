using System;
using System.Runtime.InteropServices;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        private unsafe static char* StringToChar(string s)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(s);
            return (char*)(p.ToPointer());
        }
    }
}
