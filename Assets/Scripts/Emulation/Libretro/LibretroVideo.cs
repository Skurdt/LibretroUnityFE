using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SK.Emulation.Libretro
{
    public partial class Wrapper
    {
        //[StructLayout(LayoutKind.Sequential)]
        //public class FramebufferPixel
        //{
        //    public float Alpha;
        //    public float Red;
        //    public float Green;
        //    public float Blue;
        //}

        public Texture2D Texture { get; private set; }
        //public FramebufferPixel[] FrameBuffer { get; private set; }

        private byte[] _dstPixels;

        public RenderTexture rt;

        private unsafe void RetroVideoRefreshCallback(void* data, uint width, uint height, uint pitch)
        {
            if (data == null || width == 0 || height == 0 || pitch == 0)
            {
                return;
            }

            int intWidth = Convert.ToInt32(width);
            int intHeight = Convert.ToInt32(height);

            //if (FrameBuffer == null || FrameBuffer.Length != intWidth * intHeight)
            //{
            //    FrameBuffer = new FramebufferPixel[intWidth * intHeight];
            //}

            switch (_pixelFormat)
            {
                case RetroPixelFormat.RETRO_PIXEL_FORMAT_0RGB1555:
                {
                    //FrameBuffer = null;
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

                    int[] pixelarr = new int[intWidth * intHeight];
                    Marshal.Copy((IntPtr)data, pixelarr, 0, intWidth * intHeight);
                    Color32[] color32arr = new Color32[intWidth * intHeight];

                    NativeArray<int> nativePixelArray = new NativeArray<int>(pixelarr, Allocator.TempJob);
                    NativeArray<Color32> nativeColorArray = new NativeArray<Color32>(color32arr, Allocator.TempJob);

                    JobHandle jobHandle = new RGB8888Job
                    {
                        PixelArray = nativePixelArray,
                        Color32Array = nativeColorArray
                    }.Schedule(pixelarr.Length, 1000);

                    jobHandle.Complete();
                    nativeColorArray.CopyTo(color32arr);

                    nativePixelArray.Dispose();
                    nativeColorArray.Dispose();

                    Texture.SetPixels32(color32arr, 0);
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

                    int dstSize = 2 * (Convert.ToInt32(pitch) * intHeight);

                    if (_dstPixels == null || _dstPixels.Length != dstSize)
                    {
                        _dstPixels = new byte[dstSize];
                    }

                    using (UnmanagedMemoryStream readStream = new UnmanagedMemoryStream((byte*)data, dstSize, dstSize, FileAccess.Read))
                    {
                        try
                        {
                            _ = readStream.Read(_dstPixels, 0, dstSize);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }

                    Texture.LoadRawTextureData(_dstPixels);
                    Texture.Apply();
                }
                break;
                default:
                {
                    //FrameBuffer = null;
                    Texture = null;
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

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
