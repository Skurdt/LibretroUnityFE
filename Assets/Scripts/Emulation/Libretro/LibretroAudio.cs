using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        private void RetroAudioSampleCallback(short left, short right)
        {
            if (_audioSource == null || _audioSource.AudioBatch == null)
            {
                return;
            }

            // Unused.
            float value = left * -0.000030517578125f;
            value = Mathf.Clamp(value, -1.0f, 1.0f);
            _audioSource.AudioBatch.Add(value);

            value = right * -0.000030517578125f;
            value = Mathf.Clamp(value, -1.0f, 1.0f);
            _audioSource.AudioBatch.Add(value);
        }

        private unsafe void RetroAudioSampleBatchCallback(short* data, uint frames)
        {
            if (_audioSource == null || _audioSource.AudioBatch == null)
            {
                return;
            }

            for (int i = 0; i < frames * 2; ++i)
            {
                float value = data[i] * 0.000030517578125f;
                value = Mathf.Clamp(value, -1.0f, 1.0f); // Unity's audio only takes values between -1 and 1.
                _audioSource.AudioBatch.Add(value);
            }
        }

        private static void SetAudioSampleRate(int sampleRate)
        {
            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            audioConfig.sampleRate = sampleRate;
            _ = AudioSettings.Reset(audioConfig);
        }
    }
}
