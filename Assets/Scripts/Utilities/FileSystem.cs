using System;
using System.IO;

namespace SK.Utilities
{
    public static class FileSystem
    {
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static bool CreateFile(string path)
        {
            bool result = false;

            try
            {
                using (_ = File.Create(GetAlsolutePath(path)))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "FileSystem.CreateFile");
            }

            return result;
        }

        public static bool DeleteFile(string path)
        {
            bool result = false;

            try
            {
                File.Delete(GetAlsolutePath(path));
#if UNITY_EDITOR
                File.Delete(GetAlsolutePath($"{path}.meta"));
#endif
                result = true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "FileSystem.DeleteFile");
            }

            return result;
        }

        public static string GetAlsolutePath(string path)
        {
            string result = null;

            try
            {
                if (path.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                {
                    result = Path.GetFullPath(Path.Combine(UnityEngine.Application.streamingAssetsPath, path.Remove(0, 1)));
                }
                else
                {
                    result = Path.GetFullPath(path);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "FileSystem.GetAlsolutePath");
            }

            return result;
        }

        public static string GetRelativePath(string path)
        {
            string result = null;

            try
            {
                result = Path.GetFullPath(path).Replace($"{UnityEngine.Application.streamingAssetsPath}{Path.DirectorySeparatorChar}", "@");
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "FileSystem.GetRelativePath");
            }

            return result;
        }

        public static string[] GetFilesInDirectory(string path, string searchPattern, bool includeSubFolders = false)
        {
            string[] result = null;

            try
            {
                result = Directory.GetFiles(GetAlsolutePath(path), searchPattern, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, "FileSystem.GetFilesInDirectory");
            }

            return result;
        }

        public static bool SerializeToJson<T>(T sourceObject, string targetPath)
        {
            bool result = false;

            try
            {
                string jsonString = UnityEngine.JsonUtility.ToJson(sourceObject, true);
                File.WriteAllText(GetAlsolutePath(targetPath), jsonString);
                result = true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, $"FileSystem.SerializeToJson<{typeof(T)}>");
            }

            return result;
        }

        public static T DeserializeFromJson<T>(string sourcePath) where T : class
        {
            T result = null;

            try
            {
                string jsonString = File.ReadAllText(GetAlsolutePath(sourcePath));
                result = UnityEngine.JsonUtility.FromJson<T>(jsonString);
            }
            catch (Exception e)
            {
                Log.Exception(e.Message, $"FileSystem.DeserializeFromJson<{typeof(T)}>");
            }

            return result;
        }
    }
}
