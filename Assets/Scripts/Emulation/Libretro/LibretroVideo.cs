using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        public Texture2D Texture { get; private set; }

        private byte[] _dstPixels565;

        private int[] _srcPixels8888;
        private Color32[] _dstPixels8888;

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

                    int arrayLength = intWidth * intHeight;
                    if (_srcPixels8888 == null || _srcPixels8888.Length != arrayLength)
                    {
                        _srcPixels8888 = new int[arrayLength];
                    }
                    Marshal.Copy((IntPtr)data, _srcPixels8888, 0, arrayLength);

                    if (_dstPixels8888 == null || _dstPixels8888.Length != arrayLength)
                    {
                        _dstPixels8888 = new Color32[arrayLength];
                    }

                    NativeArray<int> nativePixelArray = new NativeArray<int>(_srcPixels8888, Allocator.TempJob);
                    NativeArray<Color32> nativeColorArray = new NativeArray<Color32>(_dstPixels8888, Allocator.TempJob);

                    JobHandle jobHandle = new RGB8888Job
                    {
                        PixelArray = nativePixelArray,
                        Color32Array = nativeColorArray
                    }.Schedule(_srcPixels8888.Length, 1000);

                    jobHandle.Complete();
                    nativeColorArray.CopyTo(_dstPixels8888);

                    nativePixelArray.Dispose();
                    nativeColorArray.Dispose();

                    Texture.SetPixels32(_dstPixels8888, 0);
                    Texture.Apply();
                }
                break;
                case RetroPixelFormat.RETRO_PIXEL_FORMAT_RGB565:
                {
                    if (Texture == null || Texture.format != TextureFormat.RGB565 || Texture.height != intHeight || Texture.width != intWidth)
                    {
                        Texture = new Texture2D(intWidth, intHeight, TextureFormat.RGB565, false)
                        {
                            filterMode = FilterMode.Trilinear
                        };
                    }

                    int dstSize = 2 * (intWidth * intHeight);
                    if (_dstPixels565 == null || _dstPixels565.Length != dstSize)
                    {
                        _dstPixels565 = new byte[dstSize];
                    }

                    byte* ptr = (byte*)data;
                    int m565 = 0;
                    for (uint y = 0; y < height; y++)
                    {
                        for (uint x = y * pitch; x < width * 2 + y * pitch; x++)
                        {
                            _dstPixels565[m565] = ptr[x];
                            m565++;
                        }
                    }

                    Texture.LoadRawTextureData(_dstPixels565);
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
        private struct RGB8888Job : IJobParallelFor
        {
            public NativeArray<int> PixelArray;
            public NativeArray<Color32> Color32Array;

            public void Execute(int index)
            {
                int packed = PixelArray[index];
                Color32Array[index] = new Color32((byte)((packed >> 16) & 0x00FF), (byte)((packed >> 8) & 0x00FF), (byte)(packed & 0x00FF), (byte)((packed >> 24) & 0x00FF));
            }
        }
    }
}
