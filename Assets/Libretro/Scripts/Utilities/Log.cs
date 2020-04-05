using System;
using System.Reflection;
using UnityEngine;

namespace SK.Libretro.Utilities
{
    public static class Log
    {
        public static void Info(string message, string caller = null)
        {
            LogInternal("<color=yellow>[INFO]</color>", message, caller);
        }

        public static void Success(string message, string caller = null)
        {
            LogInternal("<color=green>[SUCCESS]</color>", message, caller);
        }

        public static void Warning(string message, string caller = null)
        {
            LogInternal("<color=orange>[WARNING]</color>", message, caller);
        }

        public static void Error(string message, string caller = null)
        {
            LogInternal("<color=red>[ERROR]</color>", message, caller);
        }

        public static void Exception(Exception e, string caller = null)
        {
            LogInternal("<color=red>[EXCEPTION]</color>", e.Message, caller);
        }

        private static void LogInternal(string prefix, string message, string caller)
        {
            Debug.Log($"{prefix} {(string.IsNullOrEmpty(caller) ? "" : $"<color=lightblue>[{caller}]</color> ")}{message}");
        }
        public static void ClearConsole()
        {
#if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            _ = method.Invoke(new object(), null);
#endif
        }
    }
}
