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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public bool OptionCropOverscan
        {
            get => _optionCropOverscan;
            set
            {
                if (_optionCropOverscan != value)
                {
                    _optionCropOverscan = value;
                    _dirtyVariables = true;
                }
            }
        }

        public bool ForceQuit { get; private set; } = false;

        public IGraphicsProcessor GraphicsProcessor { get; private set; }
        public IAudioProcessor AudioProcessor { get; private set; }
        public IInputProcessor InputProcessor { get; private set; }

        public readonly TargetPlatform TargetPlatform;

        public static string WrapperDirectory;
        public static string CoresDirectory;
        public static string SystemDirectory;
        public static string CoreAssetsDirectory;
        public static string SavesDirectory;
        public static string TempDirectory;
        public static string CoreOptionsFile;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private readonly List<IntPtr> _unsafeStrings = new List<IntPtr>();

        private CoreOptionsList _coreOptionsList;
        private bool _optionCropOverscan = true;
        private bool _dirtyVariables     = true;
        private bool _glSupport;

        public Wrapper(TargetPlatform targetPlatform, string baseDirectory = null)
        {
            TargetPlatform = targetPlatform;

            if (WrapperDirectory == null)
            {
                WrapperDirectory    = !string.IsNullOrEmpty(baseDirectory) ? baseDirectory : "libretro~";
                CoresDirectory      = $"{WrapperDirectory}/cores";
                SystemDirectory     = $"{WrapperDirectory}/system";
                CoreAssetsDirectory = $"{WrapperDirectory}/core_assets";
                SavesDirectory      = $"{WrapperDirectory}/saves";
                TempDirectory       = $"{WrapperDirectory}/temp";
                CoreOptionsFile     = $"{WrapperDirectory}/core_options.json";
            }

            string wrapperDirectory = FileSystem.GetAbsolutePath(WrapperDirectory);
            if (!Directory.Exists(wrapperDirectory))
            {
                _ = Directory.CreateDirectory(wrapperDirectory);
            }

            string tempDirectory = FileSystem.GetAbsolutePath(TempDirectory);
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }

            // InitGL();
        }

        public bool StartGame(string coreName, string gameDirectory, string gameName)
        {
            LoadCoreOptionsFile();

            if (!Core.Start(this, coreName))
            {
                return false;
            }

            if (!Game.Start(Core, gameDirectory, gameName))
            {
                return false;
            }

            return true;
        }

        public void StopGame()
        {
            AudioProcessor?.DeInit();

            Game.Stop();
            Core.Stop();

            glfwDestroyWindow(_windowHandle);
            glfwTerminate();

            for (int i = 0; i < _unsafeStrings.Count; ++i)
            {
                Marshal.FreeHGlobal(_unsafeStrings[i]);
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public void Update()
        {
            if (ForceQuit || !Game.Running || !Core.Initialized)
            {
                return;
            }

            // FIXME(Tom):
            // An AccessViolationException get thrown by the core when files (roms, bios, etc...) are missing and probably for other various reasons...
            // In a normal C# project, this get captured here (when using the attributes) and errors can be displayed properly.
            // Unity simply crashes here but we only know about the missing things after a call to retro_run...
            try
            {
                Core.retro_run();
            }
            catch (AccessViolationException e)
            {
                Log.Exception(e);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public void ActivateGraphics(IGraphicsProcessor graphicsProcessor) => GraphicsProcessor = graphicsProcessor;

        public void DeactivateGraphics() => GraphicsProcessor = null;

        public void ActivateAudio(IAudioProcessor audioProcessor)
        {
            AudioProcessor = audioProcessor;
            AudioProcessor?.Init((int)Game.SystemAVInfo.timing.sample_rate);
        }

        public void DeactivateAudio()
        {
            AudioProcessor?.DeInit();
            AudioProcessor = null;
        }

        public void ActivateInput(IInputProcessor inputProcessor) => InputProcessor = inputProcessor;

        public void DeactivateInput() => InputProcessor = null;

        private void LoadCoreOptionsFile()
        {
            _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
            if (_coreOptionsList == null)
            {
                _coreOptionsList = new CoreOptionsList();
            }
        }

        private void SaveCoreOptionsFile()
        {
            if (_coreOptionsList == null || _coreOptionsList.Cores.Count == 0)
            {
                return;
            }

            _coreOptionsList.Cores = _coreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
            for (int i = 0; i < _coreOptionsList.Cores.Count; ++i)
            {
                _coreOptionsList.Cores[i].Options.Sort();
            }
            _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
        }

        // GLFW
        [DllImport("glfw3")] static extern IntPtr glfwGetProcAddress(string procname);
        [DllImport("glfw3")] static extern bool glfwInit();
        [DllImport("glfw3")] static extern void glfwTerminate();
        [DllImport("glfw3")] static extern IntPtr glfwCreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);
        [DllImport("glfw3")] static extern void glfwDestroyWindow(IntPtr window);
        [DllImport("glfw3")] static extern void glfwMakeContextCurrent(IntPtr window);
        [DllImport("glfw3")] static extern bool glfwWindowShouldClose(IntPtr window);
        [DllImport("glfw3")] static extern void glfwPollEvents();
        [DllImport("glfw3")] static extern void glfwSwapBuffers(IntPtr window);

        //TODO(Tom): Make this work and move code to somewhere else when (if ever...) working

        // GL
        const uint GL_FRAMEBUFFER                               = 0x8D40;
        const uint GL_TEXTURE_2D                                = 0x0DE1;
        const uint GL_RGBA8                                     = 0x8058;
        const uint GL_COLOR_ATTACHMENT0                         = 0x8CE0;
        const uint GL_RENDERBUFFER                              = 0x8D41;
        const uint GL_DEPTH_COMPONENT16                         = 0x81A5;
        const uint GL_DEPTH_ATTACHMENT                          = 0x8D00;
        const uint GL_FRAMEBUFFER_COMPLETE                      = 0x8CD5;
        const uint GL_FRAMEBUFFER_UNDEFINED                     = 0x8219;
        const uint GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT         = 0x8CD6;
        const uint GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
        const uint GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER        = 0x8CDB;
        const uint GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER        = 0x8CDC;
        const uint GL_FRAMEBUFFER_UNSUPPORTED                   = 0x8CDD;
        const uint GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE        = 0x8D56;
        const uint GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS      = 0x8DA8;
        const uint GL_COLOR_BUFFER_BIT                          = 0x00004000;
        const uint GL_DEPTH_BUFFER_BIT                          = 0x00000100;

        unsafe delegate void glGenFramebuffers_f(int n, uint* ids);
        delegate void glBindFramebuffer_f(uint target, uint framebuffer);
        unsafe delegate void glGenTextures_f(uint n, uint* textures);
        delegate void glBindTexture_f(uint target, uint texture);
        delegate void glTexStorage2D_f(uint target, int levels, uint internalformat, int width, int height);
        delegate void glFramebufferTexture2D_f(uint target, uint attachment, uint textarget, uint texture, int level);
        unsafe delegate void glGenRenderbuffers_f(int n, uint* renderbuffers);
        delegate void glBindRenderbuffer_f(uint target, uint renderbuffer);
        delegate void glRenderbufferStorage_f(uint target, uint internalformat, int width, int height);
        delegate void glFramebufferRenderbuffer_f(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer);
        delegate uint glCheckFramebufferStatus_f(uint target);
        delegate void glClear_f(uint flags);

        glGenFramebuffers_f glGenFramebuffers;
        glBindFramebuffer_f glBindFramebuffer;
        glGenTextures_f glGenTextures;
        glBindTexture_f glBindTexture;
        glTexStorage2D_f glTexStorage2D;
        glFramebufferTexture2D_f glFramebufferTexture2D;
        glGenRenderbuffers_f glGenRenderbuffers;
        glBindRenderbuffer_f glBindRenderbuffer;
        glRenderbufferStorage_f glRenderbufferStorage;
        glFramebufferRenderbuffer_f glFramebufferRenderbuffer;
        glCheckFramebufferStatus_f glCheckFramebufferStatus;
        glClear_f glClear;

        // My stuff...
        IntPtr GetProcAddress(string procname) => glfwGetProcAddress(procname);
        IntPtr _windowHandle;
        retro_hw_render_callback _hwRenderCallback;
        readonly uint[] _framebuffers = new uint[32];
        readonly uint[] _renderbuffers = new uint[32];
        readonly uint[] _textures = new uint[32];

        retro_hw_get_current_framebuffer_t _videoDriverGetCurrentFrameBufferCallback;
        retro_hw_get_proc_address_t _videoDriverGetProcAddressCallback;

        uint VideoDriverGetCurrentFrameBuffer() => _framebuffers[0];

        private unsafe void InitGL()
        {
            if (!glfwInit())
            {
                _glSupport = false;
                return;
            }

            _windowHandle = glfwCreateWindow(640, 480, "Testing", IntPtr.Zero, IntPtr.Zero);
            if (_windowHandle == IntPtr.Zero)
            {
                _glSupport = false;
                glfwTerminate();
                return;
            }

            glfwMakeContextCurrent(_windowHandle);

            glGenFramebuffers         = Marshal.GetDelegateForFunctionPointer<glGenFramebuffers_f>(glfwGetProcAddress("glGenFramebuffers"));
            glBindFramebuffer         = Marshal.GetDelegateForFunctionPointer<glBindFramebuffer_f>(glfwGetProcAddress("glBindFramebuffer"));
            glGenTextures             = Marshal.GetDelegateForFunctionPointer<glGenTextures_f>(glfwGetProcAddress("glGenTextures"));
            glBindTexture             = Marshal.GetDelegateForFunctionPointer<glBindTexture_f>(glfwGetProcAddress("glBindTexture"));
            glTexStorage2D            = Marshal.GetDelegateForFunctionPointer<glTexStorage2D_f>(glfwGetProcAddress("glTexStorage2D"));
            glFramebufferTexture2D    = Marshal.GetDelegateForFunctionPointer<glFramebufferTexture2D_f>(glfwGetProcAddress("glFramebufferTexture2D"));
            glGenRenderbuffers        = Marshal.GetDelegateForFunctionPointer<glGenRenderbuffers_f>(glfwGetProcAddress("glGenRenderbuffers"));
            glBindRenderbuffer        = Marshal.GetDelegateForFunctionPointer<glBindRenderbuffer_f>(glfwGetProcAddress("glBindRenderbuffer"));
            glRenderbufferStorage     = Marshal.GetDelegateForFunctionPointer<glRenderbufferStorage_f>(glfwGetProcAddress("glRenderbufferStorage"));
            glFramebufferRenderbuffer = Marshal.GetDelegateForFunctionPointer<glFramebufferRenderbuffer_f>(glfwGetProcAddress("glFramebufferRenderbuffer"));
            glCheckFramebufferStatus  = Marshal.GetDelegateForFunctionPointer<glCheckFramebufferStatus_f>(glfwGetProcAddress("glCheckFramebufferStatus"));
            glClear = Marshal.GetDelegateForFunctionPointer<glClear_f>(glfwGetProcAddress("glClear"));

            fixed (uint* ptr = _framebuffers)
            {
                glGenFramebuffers(1, ptr);
            }

            glBindFramebuffer(GL_FRAMEBUFFER, _framebuffers[0]);

            fixed (uint* ptr = _textures)
            {
                glGenTextures(1, ptr);
            }

            glBindTexture(GL_TEXTURE_2D, _textures[0]);
            glTexStorage2D(GL_TEXTURE_2D, 1, GL_RGBA8, 640, 480);
            glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, _textures[0], 0);

            fixed (uint* ptr = _renderbuffers)
            {
                glGenRenderbuffers(1, ptr);
            }

            glBindRenderbuffer(GL_RENDERBUFFER, _renderbuffers[0]);
            glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT16, 640, 480);
            glBindRenderbuffer(GL_RENDERBUFFER, 0);
            glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, _renderbuffers[0]);

            uint status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
            if (status != GL_FRAMEBUFFER_COMPLETE)
            {
                _glSupport = false;
                _windowHandle = IntPtr.Zero;
                glfwDestroyWindow(_windowHandle);
                glfwTerminate();
                return;
            }

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            glBindTexture(GL_TEXTURE_2D, 0);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);

            _videoDriverGetCurrentFrameBufferCallback = VideoDriverGetCurrentFrameBuffer;
            _videoDriverGetProcAddressCallback = glfwGetProcAddress;

            _glSupport = true;
        }
    }
}
