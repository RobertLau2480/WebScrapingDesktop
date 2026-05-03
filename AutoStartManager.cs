using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;

namespace WebScrapingDesktop
{
    /// <summary>
    /// 管理应用程序的「开机自启动」注册表项（HKCU\Software\Microsoft\Windows\CurrentVersion\Run）
    /// </summary>
    public static class AutoStartManager
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string AppName = Branding.AppName;

        /// <summary>
        /// 可靠获取当前 exe 的真实路径（发布后为 .exe；开发中 dotnet run 时获取不准属正常）
        /// </summary>
        private static string ExePath
        {
            get
            {
                // 优先使用进程路径，其次回退到 EntryAssembly.Location，最后用 MainModule.FileName
                return Environment.ProcessPath
                       ?? Assembly.GetEntryAssembly()?.Location
                       ?? Process.GetCurrentProcess().MainModule?.FileName
                       ?? string.Empty;
            }
        }

        /// <summary>
        /// 设置是否开机自启。enable=true 时写入注册表，false 时删除。
        /// </summary>
        public static void SetAutoStart(bool enable)
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, true))
            {
                if (key == null) return;

                if (enable)
                {
                    // 路径加双引号，防止空格导致启动失败
                    string value = $"\"{ExePath}\"";
                    key.SetValue(AppName, value);
                }
                else
                {
                    if (key.GetValue(AppName) != null)
                        key.DeleteValue(AppName, false);
                }
            }
        }

        /// <summary>
        /// 检查当前是否已设置为开机自启。
        /// </summary>
        public static bool IsAutoStartEnabled()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, false))
            {
                return key?.GetValue(AppName) != null;
            }
        }
    }
}