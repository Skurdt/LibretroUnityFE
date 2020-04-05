using System.Collections.Generic;
using UnityEngine;

namespace SK.Libretro
{
    [RequireComponent(typeof(AudioSource))]
    public class UnityAudioProcessorComponent : MonoBehaviour, IAudioProcessor
    {
        private const int AUDIO_BUFFER_SIZE = 65536;

        private AudioSource _audioSource;
        private readonly List<float> _audioBuffer = new List<float>(AUDIO_BUFFER_SIZE);

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_audioBuffer != null && _audioBuffer.Count >= data.Length)
            {
                _audioBuffer.CopyTo(0, data, 0, data.Length);
                _audioBuffer.RemoveRange(0, data.Length);
            }
        }

        public void Init(int sampleRate)
        {
            DeInit();

            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            audioConfig.sampleRate = sampleRate;
            _ = AudioSettings.Reset(audioConfig);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = AudioClip.Create("LibretroAudioClip", AUDIO_BUFFER_SIZE, 2, sampleRate, false);
            _audioSource.Play();
        }

        public void DeInit()
        {
            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            _audioBuffer.Clear();
        }

        public void ProcessSamples(float[] samples)
        {
            _audioBuffer.AddRange(samples);
        }
    }
}
