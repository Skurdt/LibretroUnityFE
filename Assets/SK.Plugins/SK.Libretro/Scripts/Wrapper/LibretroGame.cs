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
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.Wrapper;

namespace SK.Libretro
{
    public class LibretroGame
    {
        public string Name { get; private set; }
        public bool Running { get; private set; }

        public retro_system_av_info SystemAVInfo;
        public retro_pixel_format PixelFormat;

        private retro_game_info _gameInfo;

        private LibretroCore _core;

        private string _extractedPath = null;

        public bool Start(LibretroCore core, string gameDirectory, string gameName)
        {
            _core = core;
            Name  = gameName;

            try
            {
                string directory = FileSystem.GetAbsolutePath(gameDirectory);
                string gamePath  = GetGamePath(directory, gameName);
                if (gamePath == null)
                {
                    // Try Zip archive
                    // TODO(Tom): Check for any file after extraction instead of exact game name (only the archive needs to match)
                    string archivePath = FileSystem.GetAbsolutePath($"{directory}/{gameName}.zip");
                    if (File.Exists(archivePath))
                    {
                        string extractDirectory = FileSystem.GetAbsolutePath($"{TempDirectory}/extracted/{gameName}_{Guid.NewGuid()}");
                        System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractDirectory);

                        gamePath       = GetGamePath(extractDirectory, gameName);
                        _extractedPath = gamePath;
                    }
                }

                if (!GetGameInfo(gamePath, out _gameInfo))
                {
                    Log.Warning($"Game not set, running '{core.CoreName}' core only.", "Libretro.LibretroGame.Start");
                    return false;
                }

                Running = LoadGame(ref _gameInfo);
                return Running;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return false;
        }

        public void Stop()
        {
            if (Running)
            {
                _core?.retro_unload_game();
                Running = false;
            }

            if (_gameInfo.data != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_gameInfo.data);
            }

            if (!string.IsNullOrEmpty(_extractedPath) && FileSystem.FileExists(_extractedPath))
            {
                Directory.Delete(Path.GetDirectoryName(_extractedPath), true);
            }
        }

        private string GetGamePath(string directory, string gameName)
        {
            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(gameName))
            {
                return null;
            }

            foreach (string extension in _core.ValidExtensions)
            {
                string filePath = FileSystem.GetAbsolutePath($"{directory}/{gameName}.{extension}");
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        private bool GetGameInfo(string gamePath, out retro_game_info gameInfo)
        {
            if (string.IsNullOrEmpty(gamePath) && _core.SupportNoGame)
            {
                Log.Info($"Game not set, running with no game support.", "Libretro.LibretroGame.Start");
                gameInfo = new retro_game_info();
                return true;
            }

            if (!string.IsNullOrEmpty(gamePath) && FileSystem.FileExists(gamePath))
            {
                if (_core.NeedFullPath)
                {
                    gameInfo = new retro_game_info
                    {
                        path = gamePath
                    };

                    return true;
                }

                using (FileStream stream = new FileStream(gamePath, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];

                    gameInfo = new retro_game_info
                    {
                        path = gamePath,
                        size = Convert.ToUInt32(data.Length),
                        data = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf<byte>())
                    };

                    _ = stream.Read(data, 0, (int)stream.Length);
                    Marshal.Copy(data, 0, gameInfo.data, data.Length);

                    return true;
                }
            }

            gameInfo = default;
            return false;
        }

        private bool LoadGame(ref retro_game_info _gameInfo)
        {
            try
            {
                if (!_core.retro_load_game(ref _gameInfo))
                {
                    return false;
                }

                SystemAVInfo = new retro_system_av_info();
                _core.retro_get_system_av_info(ref SystemAVInfo);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e, "Libretro.LibretroGame.LoadGame");
            }

            return false;
        }
    }
}
