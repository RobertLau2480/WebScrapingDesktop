using System;
using System.Windows;

namespace WebScrapingDesktop
{
    public partial class App : Application
    {
        private MainWindow? _mainWindow;
        private TrayIconManager? _trayManager;
        private bool _isShuttingDown = false;   // 退出标志，阻止关闭事件覆盖设置

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 加载设置
            var settings = SettingsManager.Load();

            // 创建主窗口（传入设置）
            _mainWindow = new MainWindow(settings);
            _mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            _mainWindow.Left = settings.WindowLeft;
            _mainWindow.Top = settings.WindowTop;
            _mainWindow.Width = settings.WindowWidth;
            _mainWindow.Height = settings.WindowHeight;
            _mainWindow.ApplyAppearance(settings);
            _mainWindow.ApplyScrapingSettings(settings);
            _mainWindow.SetLockState(settings.IsLocked);

            // 根据设置决定是否显示窗口
            if (settings.IsVisible)
                _mainWindow.Show();
            else
                _mainWindow.Hide();

            // 拦截系统关机/注销，提前保存正确可见状态并标记退出
            this.SessionEnding += (s, args) =>
            {
                _isShuttingDown = true;
                SettingsManager.Save(settings, _mainWindow);   // 此时窗口仍处于原有显示/隐藏状态
            };

            // 主窗口关闭事件：仅隐藏，不退出程序（除非正在退出）
            _mainWindow.Closing += (s, args) =>
            {
                if (_isShuttingDown)
                    return; // 正在由系统/托盘退出，不拦截关闭，让程序自然结束

                // 用户点击窗口关闭按钮 -> 隐藏窗口并保存状态
                args.Cancel = true;
                _mainWindow.Hide();
                SettingsManager.Save(settings, _mainWindow);
            };

            // 位置/大小变化时更新设置对象（注：不立即写入文件，由托盘菜单或退出时统一保存）
            _mainWindow.LocationChanged += (_, _) => UpdateWindowBounds(settings);
            _mainWindow.SizeChanged += (_, _) => UpdateWindowBounds(settings);

            // 初始化托盘管理器
            _trayManager = new TrayIconManager(_mainWindow, settings);
            _trayManager.Initialize();
        }

        // 更新窗口位置和大小的内存数据
        private void UpdateWindowBounds(Settings settings)
        {
            if (_mainWindow != null)
            {
                settings.WindowLeft = _mainWindow.Left;
                settings.WindowTop = _mainWindow.Top;
                settings.WindowWidth = _mainWindow.Width;
                settings.WindowHeight = _mainWindow.Height;
            }
        }

        /// <summary>
        /// 托盘菜单“退出”时调用，确保最终保存一次正确的可见状态
        /// </summary>
        public void ExitApplication()
        {
            if (_isShuttingDown) return; // 防止重复调用
            _isShuttingDown = true;

            // 保存当前完整状态（窗口的可见性会被 SettingsManager.Save 正确读取）
            SettingsManager.Save(_mainWindow!.Settings, _mainWindow);
            _trayManager?.Dispose();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayManager?.Dispose();
            base.OnExit(e);
        }
    }
}