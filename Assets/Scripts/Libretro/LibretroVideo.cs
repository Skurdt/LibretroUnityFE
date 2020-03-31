using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public Texture2D Texture { get; private set; }

        private unsafe void RetroVideoRefreshCallback(void* data, uint width, uint height, uint pitch)
        {
            if (data == null || width == 0 || height == 0 || pitch == 0)
            {
                return;
            }

            int intWidth  = (int)width;
            int intHeight = (int)height;

            switch (_pixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                {
                    if (Texture == null || Texture.format != TextureFormat.BGRA32 || Texture.width != intWidth || Texture.height != intHeight)
                    {
                        Texture = new Texture2D(intWidth, intHeight, TextureFormat.BGRA32, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    new ARGB1555Job
                    {
                        SourceData = (ushort*)data,
                        Width = intWidth,
                        Height = intHeight,
                        PitchPixels = (int)(pitch / sizeof(ushort)),
                        TextureData = Texture.GetRawTextureData<uint>()
                    }.Schedule().Complete();

                    Texture.Apply();
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                {
                    if (Texture == null || Texture.format != TextureFormat.BGRA32 || Texture.width != intWidth || Texture.height != intHeight)
                    {
                        Texture = new Texture2D(intWidth, intHeight, TextureFormat.BGRA32, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    new ARGB8888Job
                    {
                        SourceData = (uint*)data,
                        Width = intWidth,
                        Height = intHeight,
                        PitchPixels = (int)(pitch / sizeof(uint)),
                        TextureData = Texture.GetRawTextureData<uint>()
                    }.Schedule().Complete();

                    Texture.Apply();
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                {
                    if (Texture == null || Texture.format != TextureFormat.RGB565 || Texture.width != intWidth || Texture.height != intHeight)
                    {
                        Texture = new Texture2D(intWidth, intHeight, TextureFormat.RGB565, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    new RGB565Job
                    {
                        SourceData = (ushort*)data,
                        Width = intWidth,
                        Height = intHeight,
                        PitchPixels = (int)(pitch / sizeof(ushort)),
                        TextureData = Texture.GetRawTextureData<ushort>()
                    }.Schedule().Complete();

                    Texture.Apply();
                }
                break;
                default:
                {
                    Texture = null;
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        [BurstCompile]
        private unsafe struct ARGB1555Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = 0; y < Height; ++y)
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
        private unsafe struct ARGB8888Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public uint* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                uint* line = SourceData;
                for (int y = 0; y < Height; ++y)
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
        private unsafe struct RGB565Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<ushort> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = line[x];
                    }
                    line += PitchPixels;
                }
            }
        }

        private static uint ARGB1555toBGRA32(ushort packed)
        {
            uint a = (uint)packed & 0x8000;
            uint r = (uint)packed & 0x7C00;
            uint g = (uint)packed & 0x03E0;
            uint b = (uint)packed & 0x1F;
            uint rgb = (r << 9) | (g << 6) | (b << 3);
            return (a * 0x1FE00) | rgb | ((rgb >> 5) & 0x070707);
        }
    }
}
