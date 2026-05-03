using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WebScrapingDesktop
{
    public partial class AppearanceSettingsWindow : Window
    {
        private readonly Settings _settings;

        public AppearanceSettingsWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;

            // 加载系统所有字体并填充下拉框
            LoadSystemFonts();

            // 加载当前设置到界面控件
            ColorHexBox.Text = _settings.MainWindowColorHex;
            OpacitySlider.Value = _settings.MainWindowOpacityPercent;
            SetColorPreview(_settings.MainWindowColorHex);

            // 文字元素1
            Font1SizeSlider.Value = _settings.TextElement1FontSize;
            Font1Color.Text = _settings.TextElement1FontColorHex;
            Font1BoldCheck.IsChecked = _settings.TextElement1IsBold;
            Font1ItalicCheck.IsChecked = _settings.TextElement1IsItalic;

            // 文字元素2
            Font2SizeSlider.Value = _settings.TextElement2FontSize;
            Font2Color.Text = _settings.TextElement2FontColorHex;
            Font2BoldCheck.IsChecked = _settings.TextElement2IsBold;
            Font2ItalicCheck.IsChecked = _settings.TextElement2IsItalic;

            // 文字元素3
            Font3SizeSlider.Value = _settings.TextElement3FontSize;
            Font3Color.Text = _settings.TextElement3FontColorHex;
            Font3BoldCheck.IsChecked = _settings.TextElement3IsBold;
            Font3ItalicCheck.IsChecked = _settings.TextElement3IsItalic;

            // 初始化文字颜色预览
            UpdateColorPreview(Font1Color.Text, Font1ColorPreview);
            UpdateColorPreview(Font2Color.Text, Font2ColorPreview);
            UpdateColorPreview(Font3Color.Text, Font3ColorPreview);
        }

        /// <summary>
        /// 获取系统所有字体名称，填充三个字体选择下拉框
        /// </summary>
        private void LoadSystemFonts()
        {
            var fontNames = Fonts.SystemFontFamilies
                                 .Select(f => f.Source)
                                 .OrderBy(name => name)
                                 .ToList();

            Font1Combo.ItemsSource = fontNames;
            Font2Combo.ItemsSource = fontNames;
            Font3Combo.ItemsSource = fontNames;

            Font1Combo.SelectedItem = fontNames.Contains(_settings.TextElement1FontFamily)
                                      ? _settings.TextElement1FontFamily
                                      : fontNames.FirstOrDefault();
            Font2Combo.SelectedItem = fontNames.Contains(_settings.TextElement2FontFamily)
                                      ? _settings.TextElement2FontFamily
                                      : fontNames.FirstOrDefault();
            Font3Combo.SelectedItem = fontNames.Contains(_settings.TextElement3FontFamily)
                                      ? _settings.TextElement3FontFamily
                                      : fontNames.FirstOrDefault();
        }

        // ========== 主窗口背景颜色相关 ==========
        private void ColorHexBox_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(ColorHexBox.Text);
                ColorPreview.Fill = new SolidColorBrush(color);
            }
            catch { }
        }

        private void SetColorPreview(string hex)
        {
            try
            {
                ColorPreview.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            }
            catch { }
        }

        private void PickColor_Click(object sender, RoutedEventArgs e)
        {
            PickColorAndSet(ColorHexBox);
        }

        // ========== 文字颜色选色按钮事件 ==========
        private void PickFont1Color_Click(object sender, RoutedEventArgs e)
        {
            PickColorAndSet(Font1Color);
        }

        private void PickFont2Color_Click(object sender, RoutedEventArgs e)
        {
            PickColorAndSet(Font2Color);
        }

        private void PickFont3Color_Click(object sender, RoutedEventArgs e)
        {
            PickColorAndSet(Font3Color);
        }

        // ========== 文字颜色文本框变化预览 ==========
        private void Font1Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateColorPreview(Font1Color.Text, Font1ColorPreview);
        }

        private void Font2Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateColorPreview(Font2Color.Text, Font2ColorPreview);
        }

        private void Font3Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateColorPreview(Font3Color.Text, Font3ColorPreview);
        }

        // ========== 通用颜色选择辅助方法 ==========
        /// <summary>
        /// 打开颜色对话框，并将选中的颜色写入目标文本框（触发了 TextChanged 事件以更新预览）
        /// </summary>
        private void PickColorAndSet(TextBox targetTextBox)
        {
            using var dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var sysColor = dialog.Color;
                Color wpfColor = Color.FromArgb(sysColor.A, sysColor.R, sysColor.G, sysColor.B);
                targetTextBox.Text = $"#{wpfColor.A:X2}{wpfColor.R:X2}{wpfColor.G:X2}{wpfColor.B:X2}";
            }
        }

        /// <summary>
        /// 根据十六进制颜色字符串更新预览 Rectangle
        /// </summary>
        private void UpdateColorPreview(string hex, System.Windows.Shapes.Rectangle previewRect)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                previewRect.Fill = new SolidColorBrush(color);
            }
            catch { }
        }

        // ========== 不透明度滑条 ==========
        private void OpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            // 暂无实时预览需求，保留空实现
        }

        // ========== 确定与取消 ==========
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            _settings.MainWindowColorHex = ColorHexBox.Text;
            _settings.MainWindowOpacityPercent = OpacitySlider.Value;

            // 文字元素1
            _settings.TextElement1FontFamily = Font1Combo.SelectedItem as string ?? "Segoe UI";
            _settings.TextElement1FontSize = Font1SizeSlider.Value;
            _settings.TextElement1FontColorHex = Font1Color.Text;
            _settings.TextElement1IsBold = Font1BoldCheck.IsChecked ?? false;
            _settings.TextElement1IsItalic = Font1ItalicCheck.IsChecked ?? false;

            // 文字元素2
            _settings.TextElement2FontFamily = Font2Combo.SelectedItem as string ?? "Segoe UI";
            _settings.TextElement2FontSize = Font2SizeSlider.Value;
            _settings.TextElement2FontColorHex = Font2Color.Text;
            _settings.TextElement2IsBold = Font2BoldCheck.IsChecked ?? false;
            _settings.TextElement2IsItalic = Font2ItalicCheck.IsChecked ?? false;

            // 文字元素3
            _settings.TextElement3FontFamily = Font3Combo.SelectedItem as string ?? "Segoe UI";
            _settings.TextElement3FontSize = Font3SizeSlider.Value;
            _settings.TextElement3FontColorHex = Font3Color.Text;
            _settings.TextElement3IsBold = Font3BoldCheck.IsChecked ?? false;
            _settings.TextElement3IsItalic = Font3ItalicCheck.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}