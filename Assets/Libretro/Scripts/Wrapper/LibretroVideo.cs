using System;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public unsafe void RetroVideoRefreshCallback(void* data, uint width, uint height, uint pitch)
        {
            if (GraphicsProcessor != null)
            {
                int intWidth = (int)width;
                int intHeight = (int)height;
                int intPitch = (int)pitch;

                switch (Game.PixelFormat)
                {
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                    {
                        GraphicsProcessor.ProcessFrame0RGB1555((ushort*)data, intWidth, intHeight, intPitch / sizeof(ushort));
                    }
                    break;
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                    {

                        GraphicsProcessor.ProcessFrameARGB8888((uint*)data, intWidth, intHeight, intPitch / sizeof(uint));
                    }
                    break;
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                    {
                        GraphicsProcessor.ProcessFrameRGB565((ushort*)data, intWidth, intHeight, intPitch / sizeof(ushort));
                    }
                    break;
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
