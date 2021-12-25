using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Core
{
    public enum ViLogLevel
    {
        TRACE = 0,
        DEBUG = 1,
        INFO = 2,
        WARN = 3,
        ERROR = 4,
        FATAL = 5
    }

    public class ViLog
    {
        public static Action<ViLogLevel, string> OnLog;
        public static volatile bool IncludeLogLevelText = true;
        public static volatile bool ExtendInfo = false;

        public static void Error(string s, [CallerFilePath] string filePath = null, [CallerLineNumber] int line = 0) // [CallerFilePath] string filePath = null,
        {
            Log(ViLogLevel.ERROR, $"{GetExtendInfo(filePath, line)}{s}");
        }
        
        public static void Info(string s, [CallerFilePath] string filePath = null, [CallerLineNumber] int line = 0) // [CallerFilePath] string filePath = null,
        {
            Log(ViLogLevel.INFO, $"{GetExtendInfo(filePath, line)}{s}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log(ViLogLevel level, string s) => OnLog.Invoke(level, $"{GetPrefix(level)}{s}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPrefix(ViLogLevel level) => IncludeLogLevelText ? $"{level.ToString()} " : string.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetExtendInfo(string filePath, int line) => ExtendInfo ? $"[{Path.GetFileNameWithoutExtension(filePath)}.cs:{line}] " : string.Empty;
    }
}