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
        public static bool UseXRGB8888Job = true;

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

                    ushort* dataPtr = (ushort*)data;
                    NativeArray<uint> textureData = Texture.GetRawTextureData<uint>();

                    for (int y = 0; y < intHeight; y++)
                    {
                        ushort* rowStart = dataPtr;
                        for (int x = 0; x < intWidth; x++)
                        {
                            ushort packed = dataPtr[x];
                            textureData[y * intWidth + x] = ARGB1555toBGRA32(packed);
                        }
                        dataPtr = rowStart + pitch / sizeof(ushort);
                    }

                    Texture.Apply();
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                {
                    int targetWidth = UseXRGB8888Job ? (int)pitch / sizeof(uint) : intWidth;

                    if (Texture == null || Texture.format != TextureFormat.BGRA32 || Texture.width != targetWidth || Texture.height != intHeight)
                    {
                        Texture = new Texture2D(targetWidth, intHeight, TextureFormat.BGRA32, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    uint* dataPtr = (uint*)data;
                    NativeArray<uint> textureData = Texture.GetRawTextureData<uint>();

                    if (UseXRGB8888Job)
                    {
                        new RGB8888Job
                        {
                            SourceData = dataPtr,
                            TextureData = textureData
                        }.Schedule(textureData.Length, 1000).Complete();
                    }
                    else
                    {
                        for (int y = 0; y < intHeight; y++)
                        {
                            uint* rowStart = dataPtr;
                            for (int x = 0; x < intWidth; x++)
                            {
                                uint packed = dataPtr[x];
                                textureData[y * intWidth + x] = ARGB8888toBGRA32(packed);
                            }
                            dataPtr = rowStart + pitch / sizeof(uint);
                        }
                    }

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

                    ushort* dataPtr = (ushort*)data;
                    NativeArray<ushort> textureData = Texture.GetRawTextureData<ushort>();

                    for (int y = 0; y < intHeight; y++)
                    {
                        ushort* rowStart = dataPtr;
                        for (int x = 0; x < intWidth; x++)
                        {
                            textureData[y * intWidth + x] = dataPtr[x];
                        }
                        dataPtr = rowStart + pitch / sizeof(ushort);
                    }

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
        private unsafe struct RGB8888Job : IJobParallelFor
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public uint* SourceData;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute(int index)
            {
                uint packed = SourceData[index];
                TextureData[index] = ARGB8888toBGRA32(packed);
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

        private static uint ARGB8888toBGRA32(uint packed)
        {
            byte a = (byte)((packed & 0x00FF0000) >> 24);
            byte r = (byte)((packed & 0x00FF0000) >> 16);
            byte g = (byte)((packed & 0x0000FF00) >> 8);
            byte b = (byte)(packed & 0x00000FF0);
            return (uint)((b) | (g << 8) | (r << 16) | (a << 24));
        }
    }
}
