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

            int intWidth = Convert.ToInt32(width);
            int intHeight = Convert.ToInt32(height);
            int intPitch = Convert.ToInt32(pitch / sizeof(uint));

            switch (_pixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                {
                    try
                    {
                        if (Texture == null || Texture.format != TextureFormat.BGRA32 || Texture.width != intPitch || Texture.height != intHeight)
                        {
                            Texture = new Texture2D(intPitch, intHeight, TextureFormat.BGRA32, false)
                            {
                                filterMode = FilterMode.Trilinear
                            };
                        }

                        short* dataPtr = (short*)data;
                        NativeArray<int> textureData = Texture.GetRawTextureData<int>();
                        int arrayLength = textureData.Length;

                        new RGB1555Job
                        {
                            SourceData = dataPtr,
                            TextureData = textureData
                        }.Schedule(arrayLength, 1000).Complete();

                        Texture.Apply();
                    }
                    catch (Exception e)
                    {
                        Utilities.Log.Exception(e.Message);
                    }
                }
                break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                {
                    try
                    {
                        if (Texture == null || Texture.format != TextureFormat.BGRA32 || Texture.width != intPitch || Texture.height != intHeight)
                        {
                            Texture = new Texture2D(intPitch, intHeight, TextureFormat.BGRA32, false)
                            {
                                filterMode = FilterMode.Trilinear
                            };
                        }

                        int* dataPtr = (int*)data;
                        NativeArray<int> textureData = Texture.GetRawTextureData<int>();
                        int arrayLength = textureData.Length;

                        new RGB8888Job
                        {
                            SourceData = dataPtr,
                            TextureData = textureData
                        }.Schedule(arrayLength, 1000).Complete();

                        Texture.Apply();
                    }
                    catch (Exception e)
                    {
                        Utilities.Log.Exception(e.Message);
                    }
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

                    byte* dataPtr = (byte*)data;
                    NativeArray<byte> textureData = Texture.GetRawTextureData<byte>();

                    int m565 = 0;
                    for (uint y = 0; y < height; y++)
                    {
                        for (uint x = y * pitch; x < width * 2 + y * pitch; x++)
                        {
                            textureData[m565] = dataPtr[x];
                            m565++;
                        }
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
        private unsafe struct RGB1555Job : IJobParallelFor
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public short* SourceData;
            [WriteOnly] public NativeArray<int> TextureData;

            public void Execute(int index)
            {
                short packed = SourceData[index * 2];
                TextureData[index] = ARGB1555toBGRA32(packed);
            }
        }

        [BurstCompile]
        private unsafe struct RGB8888Job : IJobParallelFor
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public int* SourceData;
            [WriteOnly] public NativeArray<int> TextureData;

            public void Execute(int index)
            {
                int packed = SourceData[index];
                TextureData[index] = ARGB8888toBGRA32(packed);
            }
        }

        private static int ARGB1555toBGRA32(short packed)
        {
            int a = packed & 0x8000;
            int r = packed & 0x7C00;
            int g = packed & 0x03E0;
            int b = packed & 0x1F;
            int rgb = (r << 9) | (g << 6) | (b << 3);
            return (a * 0x1FE00) | rgb | ((rgb >> 5) & 0x070707);
        }

        private static int ARGB8888toBGRA32(int packed)
        {
            byte a = (byte)((packed & 0x00FF0000) >> 24);
            byte r = (byte)((packed & 0x00FF0000) >> 16);
            byte g = (byte)((packed & 0x0000FF00) >> 8);
            byte b = (byte)(packed & 0x00000FF0);
            return (b) | (g << 8) | (r << 16) | (a << 24);
        }
    }
}
