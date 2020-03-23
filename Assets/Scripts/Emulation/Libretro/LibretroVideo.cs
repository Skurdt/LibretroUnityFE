using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace SK.Emulation.Libretro
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

            switch (_pixelFormat)
            {
                case RetroPixelFormat.RETRO_PIXEL_FORMAT_0RGB1555:
                {
                    Texture = null;
                    throw new NotImplementedException();
                }
                case RetroPixelFormat.RETRO_PIXEL_FORMAT_XRGB8888:
                {
                    if (Texture == null || Texture.format != TextureFormat.ARGB32 || Texture.height != intHeight || Texture.width != intWidth)
                    {
                        Texture = new Texture2D(intWidth, intHeight, TextureFormat.ARGB32, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    int* dataPtr = (int*)data;
                    int arrayLength = intWidth * intHeight;

                    new RGB8888Job
                    {
                        SourceData = dataPtr,
                        TextureData = Texture.GetRawTextureData<int>()
                    }.Schedule(arrayLength, 1000).Complete();

                    Texture.Apply();
                }
                break;
                case RetroPixelFormat.RETRO_PIXEL_FORMAT_RGB565:
                {
                    if (Texture == null || Texture.format != TextureFormat.RGB565|| Texture.height != intHeight || Texture.width != intWidth)
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
        private unsafe struct RGB8888Job : IJobParallelFor
        {
            [ReadOnly][NativeDisableUnsafePtrRestriction] public int* SourceData;
            [WriteOnly] public NativeArray<int> TextureData;

            public void Execute(int index)
            {
                int packed = SourceData[index];
                byte a = (byte)((packed >> 24) & 0x00FF);
                byte r = (byte)((packed >> 16) & 0x00FF);
                byte g = (byte)((packed >> 8) & 0x00FF);
                byte b = (byte)(packed & 0x00FF);
                TextureData[index] = (b << 24) | (g << 16) | (r << 8) | (a);
            }
        }
    }
}
