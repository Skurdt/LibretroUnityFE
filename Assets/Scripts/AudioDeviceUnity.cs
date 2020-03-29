using System.Collections.Generic;
using UnityEngine;

namespace SK
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioDeviceUnity : MonoBehaviour
    {
        private const int AUDIO_BUFFER_SIZE = 65536;

        private AudioSource _audioSource;
        private List<float> _audioBuffer;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Libretro.Wrapper.OnAudioCallback += UploadSamples;
            Libretro.Wrapper.OnGameStartedEvent += Init;
            Libretro.Wrapper.OnGameStoppedEvent += DeInit;
        }

        private void OnDisable()
        {
            Libretro.Wrapper.OnAudioCallback -= UploadSamples;
            Libretro.Wrapper.OnGameStartedEvent -= Init;
            Libretro.Wrapper.OnGameStoppedEvent -= DeInit;
        }

        private void Init(Libretro.Wrapper.LibretroGame game)
        {
            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            audioConfig.sampleRate = game.SampleRate;
            _ = AudioSettings.Reset(audioConfig);

            _audioBuffer = new List<float>(AUDIO_BUFFER_SIZE);
            _audioSource.clip = AudioClip.Create("LibretroAudioClip", AUDIO_BUFFER_SIZE, 2, game.SampleRate, false);
            _audioSource.Play();
        }

        private void DeInit(Libretro.Wrapper.LibretroGame game)
        {
            _audioSource.Stop();
            _audioBuffer.Clear();
            _audioBuffer = null;
        }

        private void UploadSamples(float[] samples)
        {
            if (_audioBuffer != null)
            {
                _audioBuffer.AddRange(samples);
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (_audioBuffer == null || _audioBuffer.Count < data.Length)
            {
                return;
            }

            _audioBuffer.CopyTo(0, data, 0, data.Length);
            _audioBuffer.RemoveRange(0, data.Length);
        }
    }
}
