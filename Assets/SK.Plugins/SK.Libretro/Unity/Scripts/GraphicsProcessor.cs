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

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using static SK.Libretro.Utilities.PixelConversion;

namespace SK.Libretro.Unity
{
    public class GraphicsProcessor : IGraphicsProcessor
    {
        public System.Action<Texture2D> OnTextureRecreated;

        public Texture2D Texture { get; private set; }

        public FilterMode VideoFilterMode
        {
            get => _filterMode;
            set
            {
                _filterMode = value;
                CreateTexture(Texture.width, Texture.height);
            }
        }

        private FilterMode _filterMode;
        private readonly TextureFormat _textureFormat;

        public GraphicsProcessor(int width, int height, TextureFormat textureFormat, FilterMode filterMode = FilterMode.Point)
        {
            _textureFormat = textureFormat;
            _filterMode    = filterMode;

            Texture = new Texture2D(width, height, textureFormat, false)
            {
                filterMode = filterMode
            };
        }

        public unsafe void ProcessFrame0RGB1555(ushort* data, int width, int height, int pitch)
        {
            CreateTexture(width, height);

            new ProcessFrame0RGB1555Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitch / sizeof(ushort),
                TextureData = Texture.GetRawTextureData<uint>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        public unsafe void ProcessFrameXRGB8888(uint* data, int width, int height, int pitch)
        {
            CreateTexture(width, height);

            new ProcessFrameXRGB8888Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitch / sizeof(uint),
                TextureData = Texture.GetRawTextureData<uint>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        public unsafe void ProcessFrameRGB565(ushort* data, int width, int height, int pitch)
        {
            CreateTexture(width, height);

            new ProcessFrameRGB565Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitch / sizeof(ushort),
                TextureData = Texture.GetRawTextureData<uint>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        private void CreateTexture(int width, int height)
        {
            if (Texture.width != width || Texture.height != height || Texture.filterMode != VideoFilterMode)
            {
                Texture = new Texture2D(width, height, _textureFormat, false)
                {
                    filterMode = _filterMode
                };

                OnTextureRecreated?.Invoke(Texture);
            }
        }

        [BurstCompile]
        private unsafe struct ProcessFrame0RGB1555Job : IJob
        {
            [ReadOnly, NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            public int Width;
            public int Height;
            public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = Height - 1; y >= 0; --y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = ARGB1555toBGRA32(line[x]);
                    }
                    line += PitchPixels;
                }
            }
        }

        [BurstCompile]
        private unsafe struct ProcessFrameXRGB8888Job : IJob
        {
            [ReadOnly, NativeDisableUnsafePtrRestriction] public uint* SourceData;
            public int Width;
            public int Height;
            public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                uint* line = SourceData;
                for (int y = Height - 1; y >= 0; --y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = line[x];
                    }
                    line += PitchPixels;
                }
            }
        }

        [BurstCompile]
        private unsafe struct ProcessFrameRGB565Job : IJob
        {
            [ReadOnly, NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            public int Width;
            public int Height;
            public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = Height - 1; y >= 0; --y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = RGB565toBGRA32(line[x]);
                    }
                    line += PitchPixels;
                }
            }
        }
    }
}
