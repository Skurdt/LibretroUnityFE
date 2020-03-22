using System;
using System.Runtime.InteropServices;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        // typedef bool (RETRO_CALLCONV *retro_environment_t)(unsigned cmd, void *data);
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool RetroEnvironmentDelegate(RetroEnvironment cmd, void* data);
        private RetroEnvironmentDelegate _environmentCallback;

        // typedef void (RETRO_CALLCONV *retro_video_refresh_t)(const void *data, unsigned width, unsigned height, size_t pitch);
        public unsafe delegate void RetroVideoRefreshDelegate(void* data, uint width, uint height, uint pitch);
        private RetroVideoRefreshDelegate _videoRefreshCallback;

        // typedef void (RETRO_CALLCONV *retro_audio_sample_t)(int16_t left, int16_t right);
        public delegate void RetroAudioSampleDelegate(short left, short right);
        private RetroAudioSampleDelegate _audioSampleCallback;

        // typedef size_t (RETRO_CALLCONV *retro_audio_sample_batch_t)(const int16_t *data, size_t frames);
        public unsafe delegate void RetroAudioSampleBatchDelegate(short* data, uint frames);
        private RetroAudioSampleBatchDelegate _audioSampleBatchCallback;

        // typedef void (RETRO_CALLCONV *retro_input_poll_t)(void);
        public delegate void RetroInputPollDelegate();
        private RetroInputPollDelegate _inputPollCallback;

        // typedef int16_t(RETRO_CALLCONV* retro_input_state_t)(unsigned port, unsigned device, unsigned index, unsigned id);
        public delegate short RetroInputStateDelegate(uint port, RetroDevice device, uint index, uint id);
        private RetroInputStateDelegate _inputStateCallback;

        // typedef void (RETRO_CALLCONV *retro_log_printf_t)(enum retro_log_level level, const char* fmt, ...);
        public delegate void RetroLogPrintfDelegate(RetroLogLevel log_level, string format, IntPtr args);
        private RetroLogPrintfDelegate _logPrintfCallback;
    }
}
