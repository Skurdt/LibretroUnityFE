namespace SK.Libretro
{
    public interface IAudioProcessor
    {
        void Init(int sampleRate);
        void DeInit();
        void ProcessSamples(float[] samples);
    }
}
