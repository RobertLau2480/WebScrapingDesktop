using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace WebScrapingDesktop
{
    public partial class MainWindow : Window
    {
        private readonly Settings _settings;
        private CancellationTokenSource? _autoRefreshCts;
        private bool _isLocked;

        // 暴露设置给外部
        public Settings Settings => _settings;

        public MainWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            _isLocked = settings.IsLocked;

            // 直接给控件赋初值
            TextElement1.Text = "准备就绪！";
            TextElement2.Text = "All set!";
            TextElement3.Text = "";

            this.SourceInitialized += OnSourceInitialized;
            this.Loaded += OnLoaded;
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            // 初始置底
            NativeMethods.SetBottomMost(this);
            // 应用锁定状态
            SetLockState(_isLocked);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await RefreshContentAsync();
            StartAutoRefresh();
        }

        public async Task RefreshContentAsync()
        {
            try
            {
                var result = await WebScraperService.FetchAsync(_settings.MainUrl,
                    _settings.MainXPath1, _settings.MainXPath2, _settings.MainXPath3);
                TextElement1.Text = result.Text1;
                TextElement2.Text = result.Text2;
                TextElement3.Text = result.Text3;
            }
            catch
            {
                try
                {
                    var result = await WebScraperService.FetchAsync(_settings.BackupUrl,
                        _settings.BackupXPath1, _settings.BackupXPath2, _settings.BackupXPath3);
                    TextElement1.Text = result.Text1;
                    TextElement2.Text = result.Text2;
                    TextElement3.Text = result.Text3;
                }
                catch
                {
                    TextElement1.Text = "获取失败";
                    TextElement2.Text = "——主备链接均超时";
                    TextElement3.Text = "Failed to obtain the data!";
                }
            }
        }

        private async void StartAutoRefresh()
        {
            _autoRefreshCts?.Cancel();
            _autoRefreshCts = new CancellationTokenSource();
            try
            {
                while (!_autoRefreshCts.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(_settings.RefreshIntervalMinutes), _autoRefreshCts.Token);
                    if (!_autoRefreshCts.IsCancellationRequested)
                    {
                        await RefreshContentAsync();
                    }
                }
            }
            catch (TaskCanceledException) { }
        }

        public void SetLockState(bool locked)
        {
            _isLocked = locked;
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero)
            {
                if (locked)
                {
                    // 鼠标穿透，不可移动调整大小
                    NativeMethods.MakeWindowTransparent(handle);
                    this.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    NativeMethods.MakeWindowOpaque(handle);
                    this.ResizeMode = ResizeMode.CanResizeWithGrip;
                }
            }
            // 保证始终置底
            NativeMethods.SetBottomMost(this);
        }

        public void ApplyAppearance(Settings settings)
        {
            // 窗口背景与透明度
            var color = (Color)ColorConverter.ConvertFromString(settings.MainWindowColorHex);
            double opacity = Math.Clamp(settings.MainWindowOpacityPercent / 100.0, 0.0, 1.0);
            MainBorder.Background = new SolidColorBrush(color) { Opacity = opacity };

            // 应用三个文字元素的样式（包含粗体/斜体）
            ApplyTextStyle(TextElement1, settings.TextElement1FontFamily,
                settings.TextElement1FontSize, settings.TextElement1FontColorHex,
                settings.TextElement1IsBold, settings.TextElement1IsItalic);
            ApplyTextStyle(TextElement2, settings.TextElement2FontFamily,
                settings.TextElement2FontSize, settings.TextElement2FontColorHex,
                settings.TextElement2IsBold, settings.TextElement2IsItalic);
            ApplyTextStyle(TextElement3, settings.TextElement3FontFamily,
                settings.TextElement3FontSize, settings.TextElement3FontColorHex,
                settings.TextElement3IsBold, settings.TextElement3IsItalic);
        }

        private void ApplyTextStyle(TextBlock textBlock, string fontFamily, double fontSize,
            string colorHex, bool isBold, bool isItalic)
        {
            textBlock.FontFamily = new FontFamily(fontFamily);
            textBlock.FontSize = fontSize;
            var textColor = (Color)ColorConverter.ConvertFromString(colorHex);
            textBlock.Foreground = new SolidColorBrush(textColor);
            textBlock.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            textBlock.FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;
        }

        public void ApplyScrapingSettings(Settings settings)
        {
            // 重新启动自动刷新以应用新的间隔
            StopAutoRefresh();
            StartAutoRefresh();
        }

        private void StopAutoRefresh()
        {
            _autoRefreshCts?.Cancel();
        }

        protected override void OnClosed(EventArgs e)
        {
            StopAutoRefresh();
            base.OnClosed(e);
        }
    }
}