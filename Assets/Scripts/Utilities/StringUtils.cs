using System;
using System.Runtime.InteropServices;

namespace SK.Utilities
{
    public unsafe static class StringUtils
    {
        public unsafe static char* StringToChars(string s)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(s);
            return (char*)(p.ToPointer());
        }
    }
}
