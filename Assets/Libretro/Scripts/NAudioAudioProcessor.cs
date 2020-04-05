using NAudio.Wave;
using System;

namespace SK.Libretro
{
    public class NAudioAudioProcessor : IAudioProcessor
    {
        private const int AUDIO_BUFFER_SIZE = 65536;

        private WaveOutEvent _audioDevice;
        private WaveFormat _audioFormat;
        private BufferedWaveProvider _bufferedWaveProvider;

        public void Init(int sampleRate)
        {
            DeInit();

            _audioDevice = new WaveOutEvent
            {
                DesiredLatency = 140
            };

            _audioFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

            _bufferedWaveProvider = new BufferedWaveProvider(_audioFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferLength            = AUDIO_BUFFER_SIZE
            };

            _audioDevice.Init(_bufferedWaveProvider);
            _audioDevice.Play();
        }

        public void DeInit()
        {
            _audioDevice?.Stop();
            _audioDevice?.Dispose();
        }

        public void ProcessSamples(float[] samples)
        {
            byte[] byteBuffer = new byte[samples.Length * sizeof(float)];
            Buffer.BlockCopy(samples, 0, byteBuffer, 0, byteBuffer.Length);
            _bufferedWaveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);
        }
    }
}
