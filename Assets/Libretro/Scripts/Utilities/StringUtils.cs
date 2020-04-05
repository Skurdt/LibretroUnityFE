using System;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public unsafe static class StringUtils
    {
        public unsafe static char* StringToChars(string str)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(str);
            return (char*)(p.ToPointer());
        }

        public unsafe static string CharsToString(char* str)
        {
            return Marshal.PtrToStringAnsi((IntPtr)str);
        }
    }
}
