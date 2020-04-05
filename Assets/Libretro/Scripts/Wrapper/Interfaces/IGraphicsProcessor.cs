namespace SK.Libretro
{
    public interface IGraphicsProcessor
    {
        unsafe void ProcessFrame0RGB1555(ushort* data, int width, int height, int pitchInPixels);
        unsafe void ProcessFrameARGB8888(uint* data, int width, int height, int pitchInPixels);
        unsafe void ProcessFrameRGB565(ushort* data, int width, int height, int pitchInPixels);
    }
}
