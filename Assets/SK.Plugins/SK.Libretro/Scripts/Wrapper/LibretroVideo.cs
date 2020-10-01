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

using static SK.Libretro.LibretroEnums;

namespace SK.Libretro
{
    public sealed class LibretroVideo
    {
        public IGraphicsProcessor Processor { get; internal set; }

        private readonly LibretroCore _core;
        private readonly LibretroGame _game;

        internal LibretroVideo(LibretroCore core, LibretroGame game)
        {
            _core = core;
            _game = game;
        }

        internal unsafe void Callback(void* data, uint width, uint height, uint pitch)
        {
            if (Processor == null)
            {
                return;
            }

            if (_core.HwAccelerated)
            {
                return;
            }

            if (data == null)
            {
                // TODO(Tom): Send previous dupped frame ?
                return;
            }

            switch (_game.PixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                {
                    Processor.ProcessFrame0RGB1555((ushort*)data, (int)width, (int)height, (int)pitch);
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                {
                    Processor.ProcessFrameXRGB8888((uint*)data, (int)width, (int)height, (int)pitch);
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                {
                    Processor.ProcessFrameRGB565((ushort*)data, (int)width, (int)height, (int)pitch);
                }
                break;
            }
        }
    }
}
