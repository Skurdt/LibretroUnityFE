using Unity.Mathematics;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public void RetroAudioSampleCallback(short left, short right)
        {
            if (AudioProcessor != null)
            {
                float[] floatBuffer = new float[]
                {
                    math.clamp(left * -0.000030517578125f, -1.0f, 1.0f),
                    math.clamp(right * -0.000030517578125f, -1.0f, 1.0f)
                };

                AudioProcessor.ProcessSamples(floatBuffer);
            }
        }

        public unsafe uint RetroAudioSampleBatchCallback(short* data, uint frames)
        {
            if (AudioProcessor != null)
            {
                float[] floatBuffer = new float[frames * 2];

                for (int i = 0; i < floatBuffer.Length; ++i)
                {
                    floatBuffer[i] = math.clamp(data[i] * 0.000030517578125f, -1.0f, 1.0f);
                }

                AudioProcessor.ProcessSamples(floatBuffer);
            }

            return frames;
        }
    }
}
