using System;
using System.Diagnostics;
using System.Reflection;

namespace Core
{
    public class ViStorage
    {
        public static void Initialize(Action<ViLogLevel, string> onLog)
        {
            ViLog.OnLog = onLog;
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(ViStorage)).Location);
            var version = versionInfo.FileVersion;
            ViLog.Info($"Version: {version}");
        }
    }
}