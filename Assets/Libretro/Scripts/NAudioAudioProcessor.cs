using NAudio.Wave;
using System;

namespace SK.Libretro
{
    public class NAudioAudioProcessor : IAudioProcessor
    {
        private const int AUDIO_BUFFER_SIZE = 65536;

        private IWavePlayer _audioDevice;
        private BufferedWaveProvider _bufferedWaveProvider;

        public void Init(int sampleRate)
        {
            try
            {
                DeInit();

                _audioDevice = new WaveOutEvent
                {
                    DesiredLatency = 140
                };

                WaveFormat audioFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate > 0 ? sampleRate : 44100, 2);
                _bufferedWaveProvider = new BufferedWaveProvider(audioFormat)
                {
                    DiscardOnBufferOverflow = true,
                    BufferLength            = AUDIO_BUFFER_SIZE
                };

                _audioDevice.Init(_bufferedWaveProvider);
                _audioDevice.Play();
            }
            catch (Exception e)
            {
                Utilities.Log.Exception(e);
            }
        }

        public void DeInit()
        {
            _audioDevice?.Stop();
            _audioDevice?.Dispose();
        }

        public void ProcessSamples(float[] samples)
        {
            if (_bufferedWaveProvider != null)
            {
                byte[] byteBuffer = new byte[samples.Length * sizeof(float)];
                Buffer.BlockCopy(samples, 0, byteBuffer, 0, byteBuffer.Length);
                _bufferedWaveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);
            }
        }
    }
}
