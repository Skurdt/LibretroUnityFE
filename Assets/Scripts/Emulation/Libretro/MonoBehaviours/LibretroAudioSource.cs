using UnityEngine;
using System.Collections.Generic;

namespace SK.Emulation.Libretro
{
    [RequireComponent(typeof(AudioSource))]
    public class LibretroAudioSource : MonoBehaviour
    {
        private AudioSource _audioSource;

        public List<float> AudioBatch { get; private set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            AudioBatch = new List<float>(65536);
        }

        public void StartAudio()
        {
            _audioSource.Play();
        }

        public void StopAudio()
        {
            _audioSource.Stop();
            AudioBatch = null;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // wait until enough data is available
            if (AudioBatch == null || AudioBatch.Count < data.Length)
            {
                return;
            }

            int i;
            for (i = 0; i < data.Length; ++i)
            {
                data[i] = AudioBatch[i];
            }
            // remove data from the beginning
            AudioBatch.RemoveRange(0, i);
        }

        private void OnGUI()
        {
            if (AudioBatch != null)
            {
                GUI.Label(new Rect(0f, 0f, 300f, 20f), $"AudioBatch Count: {AudioBatch.Count}");
            }
        }
    }
}
