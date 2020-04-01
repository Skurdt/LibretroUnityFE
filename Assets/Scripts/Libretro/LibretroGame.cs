using System;
using System.IO;

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

            public bool Running;

            public IntPtr internalData;

            public LibretroGame(string gamePath)
            {
                Name = Path.GetFileNameWithoutExtension(gamePath);
            }
        }
    }
}
