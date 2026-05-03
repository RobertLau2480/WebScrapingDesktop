using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace WebScrapingDesktop
{
    public class Settings
    {
        // ========== 外观 ==========
        public string MainWindowColorHex { get; set; } = "#88000000";
        public double MainWindowOpacityPercent { get; set; } = 80.0;

        // 文字元素1
        public string TextElement1FontFamily { get; set; } = "Segoe UI";
        public double TextElement1FontSize { get; set; } = 14;
        public string TextElement1FontColorHex { get; set; } = "#FFFFFF";
        public bool TextElement1IsBold { get; set; } = false;
        public bool TextElement1IsItalic { get; set; } = false;

        // 文字元素2
        public string TextElement2FontFamily { get; set; } = "Segoe UI";
        public double TextElement2FontSize { get; set; } = 14;
        public string TextElement2FontColorHex { get; set; } = "#FFFFFF";
        public bool TextElement2IsBold { get; set; } = false;
        public bool TextElement2IsItalic { get; set; } = false;

        // 文字元素3
        public string TextElement3FontFamily { get; set; } = "Segoe UI";
        public double TextElement3FontSize { get; set; } = 14;
        public string TextElement3FontColorHex { get; set; } = "#FFFFFF";
        public bool TextElement3IsBold { get; set; } = false;
        public bool TextElement3IsItalic { get; set; } = false;

        // ========== 抓取设置 ==========
        public string MainUrl { get; set; } = "https://example.com";
        public string MainXPath1 { get; set; } = "/html/body/div[1]";
        public string MainXPath2 { get; set; } = "/html/body/div[2]";
        public string MainXPath3 { get; set; } = "/html/body/div[3]";
        public string BackupUrl { get; set; } = "https://example.org";
        public string BackupXPath1 { get; set; } = "/html/body/div[1]";
        public string BackupXPath2 { get; set; } = "/html/body/div[2]";
        public string BackupXPath3 { get; set; } = "/html/body/div[3]";

        // ========== 通用 ==========
        public int RefreshIntervalMinutes { get; set; } = 5;
        public bool IsLocked { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool AutoStart { get; set; } = false;      // 新增：开机自启
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public double WindowWidth { get; set; } = 400;
        public double WindowHeight { get; set; } = 300;
    }

    public static class SettingsManager
    {
        private static readonly string SettingsPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static Settings Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                    return new Settings();

                string json = File.ReadAllText(SettingsPath);
                return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
            }
            catch
            {
                return new Settings();
            }
        }

        public static void Save(Settings settings, MainWindow window)
        {
            try
            {
                settings.WindowLeft = window.Left;
                settings.WindowTop = window.Top;
                settings.WindowWidth = window.Width;
                settings.WindowHeight = window.Height;
                settings.IsVisible = window.Visibility == Visibility.Visible;

                string content = $@"{{
    // ========== 外观设置 ==========
    ""MainWindowColorHex"": ""{settings.MainWindowColorHex}"",  // 主窗口背景色 (ARGB)
    ""MainWindowOpacityPercent"": {settings.MainWindowOpacityPercent}, // 不透明度 (0-100)
    // 文字元素1
    ""TextElement1FontFamily"": ""{settings.TextElement1FontFamily}"",
    ""TextElement1FontSize"": {settings.TextElement1FontSize},
    ""TextElement1FontColorHex"": ""{settings.TextElement1FontColorHex}"",
    ""TextElement1IsBold"": {settings.TextElement1IsBold.ToString().ToLower()},
    ""TextElement1IsItalic"": {settings.TextElement1IsItalic.ToString().ToLower()},
    // 文字元素2
    ""TextElement2FontFamily"": ""{settings.TextElement2FontFamily}"",
    ""TextElement2FontSize"": {settings.TextElement2FontSize},
    ""TextElement2FontColorHex"": ""{settings.TextElement2FontColorHex}"",
    ""TextElement2IsBold"": {settings.TextElement2IsBold.ToString().ToLower()},
    ""TextElement2IsItalic"": {settings.TextElement2IsItalic.ToString().ToLower()},
    // 文字元素3
    ""TextElement3FontFamily"": ""{settings.TextElement3FontFamily}"",
    ""TextElement3FontSize"": {settings.TextElement3FontSize},
    ""TextElement3FontColorHex"": ""{settings.TextElement3FontColorHex}"",
    ""TextElement3IsBold"": {settings.TextElement3IsBold.ToString().ToLower()},
    ""TextElement3IsItalic"": {settings.TextElement3IsItalic.ToString().ToLower()},

    // ========== 抓取设置 ==========
    ""MainUrl"": ""{settings.MainUrl}"",
    ""MainXPath1"": ""{settings.MainXPath1}"",
    ""MainXPath2"": ""{settings.MainXPath2}"",
    ""MainXPath3"": ""{settings.MainXPath3}"",
    ""BackupUrl"": ""{settings.BackupUrl}"",
    ""BackupXPath1"": ""{settings.BackupXPath1}"",
    ""BackupXPath2"": ""{settings.BackupXPath2}"",
    ""BackupXPath3"": ""{settings.BackupXPath3}"",

    // ========== 通用设置 ==========
    ""RefreshIntervalMinutes"": {settings.RefreshIntervalMinutes},  // 自动刷新间隔(分钟)
    ""IsLocked"": {settings.IsLocked.ToString().ToLower()},        // 是否锁定
    ""IsVisible"": {settings.IsVisible.ToString().ToLower()},      // 是否显示
    ""AutoStart"": {settings.AutoStart.ToString().ToLower()},      // 是否开机自启
    ""WindowLeft"": {settings.WindowLeft},
    ""WindowTop"": {settings.WindowTop},
    ""WindowWidth"": {settings.WindowWidth},
    ""WindowHeight"": {settings.WindowHeight}
}}";
                File.WriteAllText(SettingsPath, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
            }
        }
    }
}