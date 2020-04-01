using NAudio.Wave;
using System;

namespace SK
{
    public class AudioDeviceExternal
    {
        private WaveOutEvent _audioDevice;
        private WaveFormat _audioFormat;
        private BufferedWaveProvider _bufferedWaveProvider;

        public AudioDeviceExternal()
        {
            Libretro.Wrapper.OnAudioProcessSamples += UploadSamples;
            Libretro.Wrapper.OnGameStartedEvent += Init;
            Libretro.Wrapper.OnGameStoppedEvent += DeInit;
        }

        private void Init(Libretro.Wrapper.LibretroGame game)
        {
            _audioDevice = new WaveOutEvent
            {
                DesiredLatency = 120
            };

            _audioFormat = WaveFormat.CreateIeeeFloatWaveFormat(game.SampleRate, 2);

            _bufferedWaveProvider = new BufferedWaveProvider(_audioFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferLength = 65536
            };

            _audioDevice.Init(_bufferedWaveProvider);
            _audioDevice.Play();
        }

        private void DeInit(Libretro.Wrapper.LibretroGame game)
        {
            Libretro.Wrapper.OnAudioProcessSamples -= UploadSamples;
            Libretro.Wrapper.OnGameStartedEvent -= Init;
            Libretro.Wrapper.OnGameStoppedEvent -= DeInit;

            _audioDevice?.Stop();
            _audioDevice?.Dispose();
        }

        private void UploadSamples(float[] samples)
        {
            byte[] byteBuffer = new byte[samples.Length * sizeof(float)];
            Buffer.BlockCopy(samples, 0, byteBuffer, 0, byteBuffer.Length);
            _bufferedWaveProvider.AddSamples(byteBuffer, 0, byteBuffer.Length);
        }
    }
}
