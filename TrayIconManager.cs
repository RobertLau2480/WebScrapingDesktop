using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace WebScrapingDesktop
{
    public class TrayIconManager : IDisposable
    {
        private readonly MainWindow _mainWindow;
        private readonly Settings _settings;
        private TaskbarIcon? _trayIcon;
        private Icon? _appIcon;

        public TrayIconManager(MainWindow mainWindow, Settings settings)
        {
            _mainWindow = mainWindow;
            _settings = settings;
        }

        public void Initialize()
        {
            string exePath = Assembly.GetEntryAssembly()!.Location;
            _appIcon = Icon.ExtractAssociatedIcon(exePath);

            _trayIcon = new TaskbarIcon
            {
                Icon = _appIcon,
                ToolTipText = Branding.AppName,
                Visibility = Visibility.Visible
            };

            var contextMenu = new ContextMenu();

            // 显示/隐藏
            var showHideItem = new MenuItem { Header = _settings.IsVisible ? "❎隐藏" : "✅显示" };
            showHideItem.Click += (_, _) =>
            {
                if (_mainWindow.Visibility == Visibility.Visible)
                    _mainWindow.Hide();
                else
                    _mainWindow.Show();

                ((MenuItem)_trayIcon.ContextMenu.Items[0]!).Header = _mainWindow.IsVisible ? "❎隐藏" : "✅显示";
                SettingsManager.Save(_settings, _mainWindow);
            };

            // 锁定/解锁
            var lockUnlockItem = new MenuItem { Header = _settings.IsLocked ? "🔓解锁" : "🔒锁定" };
            lockUnlockItem.Click += (_, _) =>
            {
                _settings.IsLocked = !_settings.IsLocked;
                _mainWindow.SetLockState(_settings.IsLocked);
                ((MenuItem)_trayIcon.ContextMenu.Items[1]!).Header = _settings.IsLocked ? "🔓解锁" : "🔒锁定";
                SettingsManager.Save(_settings, _mainWindow);
            };

            // 刷新
            var refreshItem = new MenuItem { Header = "🔁刷新" };
            refreshItem.Click += async (_, _) => await _mainWindow.RefreshContentAsync();

            // 外观设置
            var appearanceItem = new MenuItem { Header = "✴️外观设置" };
            appearanceItem.Click += (_, _) =>
            {
                var dialog = new AppearanceSettingsWindow(_settings) { Owner = _mainWindow };
                if (dialog.ShowDialog() == true)
                {
                    _mainWindow.ApplyAppearance(_settings);
                    SettingsManager.Save(_settings, _mainWindow);
                }
            };

            // 抓取设置
            var scrapingItem = new MenuItem { Header = "🌐抓取设置" };
            scrapingItem.Click += (_, _) =>
            {
                var dialog = new ScrapingSettingsWindow(_settings) { Owner = _mainWindow };
                if (dialog.ShowDialog() == true)
                {
                    _mainWindow.ApplyScrapingSettings(_settings);
                    SettingsManager.Save(_settings, _mainWindow);
                }
            };

            // ******** 开机自启（新增）********
            string autoStartHeader = _settings.AutoStart ? "⚙️开机自启：开" : "⚙️开机自启：关";
            var autoStartItem = new MenuItem { Header = autoStartHeader };
            autoStartItem.Click += (_, _) =>
            {
                _settings.AutoStart = !_settings.AutoStart;
                AutoStartManager.SetAutoStart(_settings.AutoStart);
                autoStartItem.Header = _settings.AutoStart ? "⚙️开机自启：开" : "⚙️开机自启：关";
                SettingsManager.Save(_settings, _mainWindow);
            };

            // 关于
            var aboutItem = new MenuItem { Header = "ℹ️关于" };
            aboutItem.Click += (_, _) =>
            {
                var about = new AboutWindow { Owner = _mainWindow };
                about.ShowDialog();
            };

            // 退出
            var exitItem = new MenuItem { Header = "❌退出" };
            exitItem.Click += (_, _) => (Application.Current as App)?.ExitApplication();

            // 按顺序添加菜单项
            contextMenu.Items.Add(showHideItem);
            contextMenu.Items.Add(lockUnlockItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(refreshItem);
            contextMenu.Items.Add(appearanceItem);
            contextMenu.Items.Add(scrapingItem);
            contextMenu.Items.Add(autoStartItem);    // 在“关于”之前
            contextMenu.Items.Add(aboutItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenu = contextMenu;
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
            _appIcon?.Dispose();
        }
    }
}