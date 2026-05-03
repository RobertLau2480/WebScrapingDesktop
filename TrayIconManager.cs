using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WebScrapingDesktop
{
    public class TrayIconManager : IDisposable
    {
        private readonly MainWindow _mainWindow;
        private readonly Settings _settings;
        private TaskbarIcon? _trayIcon;

        public TrayIconManager(MainWindow mainWindow, Settings settings)
        {
            _mainWindow = mainWindow;
            _settings = settings;
        }

        public void Initialize()
        {
            _trayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon("logo.ico"),
                ToolTipText = Branding.AppName,
                Visibility = Visibility.Visible
            };

            // 右击菜单
            var contextMenu = new ContextMenu();
            var showHideItem = new MenuItem { Header = _settings.IsVisible ? "❎隐藏" : "✅显示" };
            showHideItem.Click += (_, _) =>
            {
                if (_mainWindow.Visibility == Visibility.Visible)
                {
                    _mainWindow.Hide();
                }
                else
                {
                    _mainWindow.Show();
                }
                // 更新菜单文字，然后保存（IsVisible 会在 Save 里自动取自窗口）
                ((MenuItem)_trayIcon.ContextMenu.Items[0]!).Header = _mainWindow.IsVisible ? "❎隐藏" : "✅显示";
                SettingsManager.Save(_settings, _mainWindow);
            };

            var lockUnlockItem = new MenuItem { Header = _settings.IsLocked ? "🔓解锁" : "🔒锁定" };
            lockUnlockItem.Click += (_, _) =>
            {
                _settings.IsLocked = !_settings.IsLocked;
                _mainWindow.SetLockState(_settings.IsLocked);
                ((MenuItem)_trayIcon.ContextMenu.Items[1]!).Header = _settings.IsLocked ? "🔓解锁" : "🔒锁定";
                SettingsManager.Save(_settings, _mainWindow);
            };

            var refreshItem = new MenuItem { Header = "🔁刷新" };
            refreshItem.Click += async (_, _) => await _mainWindow.RefreshContentAsync();

            var appearanceItem = new MenuItem { Header = "✴️外观设置" };
            appearanceItem.Click += (_, _) =>
            {
                var dialog = new AppearanceSettingsWindow(_settings);
                dialog.Owner = _mainWindow;
                if (dialog.ShowDialog() == true)
                {
                    _mainWindow.ApplyAppearance(_settings);
                    SettingsManager.Save(_settings, _mainWindow);
                }
            };

            var scrapingItem = new MenuItem { Header = "🌐抓取设置" };
            scrapingItem.Click += (_, _) =>
            {
                var dialog = new ScrapingSettingsWindow(_settings);
                dialog.Owner = _mainWindow;
                if (dialog.ShowDialog() == true)
                {
                    _mainWindow.ApplyScrapingSettings(_settings);
                    SettingsManager.Save(_settings, _mainWindow);
                }
            };

            var aboutItem = new MenuItem { Header = "ℹ️关于" };
            aboutItem.Click += (_, _) =>
            {
                var about = new AboutWindow();
                about.Owner = _mainWindow;
                about.ShowDialog();
            };

            var exitItem = new MenuItem { Header = "❌退出" };
            exitItem.Click += (_, _) =>
            {
                (Application.Current as App)?.ExitApplication();
            };

            contextMenu.Items.Add(showHideItem);
            contextMenu.Items.Add(lockUnlockItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(refreshItem);
            contextMenu.Items.Add(appearanceItem);
            contextMenu.Items.Add(scrapingItem);
            contextMenu.Items.Add(aboutItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenu = contextMenu;
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }
    }
}