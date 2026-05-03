using Microsoft.Win32;
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
        private static readonly string ExePath = Assembly.GetEntryAssembly()!.Location;

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
                    key.SetValue(AppName, ExePath);
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