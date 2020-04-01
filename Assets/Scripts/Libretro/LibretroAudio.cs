using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public delegate void ProcessAudioSamplesDelegate(float[] samples);
        public static event ProcessAudioSamplesDelegate OnAudioProcessSamples;

        private void RetroAudioSampleCallback(short left, short right)
        {
            float[] floatBuffer = new float[]
            {
                Mathf.Clamp(left * -0.000030517578125f, -1.0f, 1.0f),
                Mathf.Clamp(right * -0.000030517578125f, -1.0f, 1.0f)
            };

            OnAudioProcessSamples?.Invoke(floatBuffer);
        }

        private unsafe uint RetroAudioSampleBatchCallback(short* data, uint frames)
        {
            float[] floatBuffer = new float[frames * 2];
            for (int i = 0; i < floatBuffer.Length; ++i)
            {
                float value = data[i] * 0.000030517578125f;
                value = Mathf.Clamp(value, -1.0f, 1.0f);
                floatBuffer[i] = value;
            }

            OnAudioProcessSamples?.Invoke(floatBuffer);

            return frames;
        }
    }
}
