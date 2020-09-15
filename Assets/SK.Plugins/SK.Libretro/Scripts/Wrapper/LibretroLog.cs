/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using SK.Libretro.Utilities;
using System;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private const string LOG_PRINTF_CALLER = "Libretro.Wrapper.RetroLogPrintf";

        public unsafe void RetroLogPrintf(retro_log_level log_level, string format, IntPtr _/*args*/)
        {
            if (log_level <= retro_log_level.RETRO_LOG_INFO)
            {
                return;
            }

            switch (log_level)
            {
                case retro_log_level.RETRO_LOG_WARN:
                    Log.Warning(format, LOG_PRINTF_CALLER);
                    break;
                case retro_log_level.RETRO_LOG_ERROR:
                    Log.Error(format, LOG_PRINTF_CALLER);
                    break;
                case retro_log_level.RETRO_LOG_DEBUG:
                case retro_log_level.RETRO_LOG_INFO:
                default:
                    Log.Info(format, LOG_PRINTF_CALLER);
                    break;
            }
        }
    }
}
