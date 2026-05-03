using System;
using System.Windows;

namespace WebScrapingDesktop
{
    public partial class ScrapingSettingsWindow : Window
    {
        private readonly Settings _settings;

        public ScrapingSettingsWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;

            MainUrlBox.Text = _settings.MainUrl;
            MainXp1Box.Text = _settings.MainXPath1;
            MainXp2Box.Text = _settings.MainXPath2;
            MainXp3Box.Text = _settings.MainXPath3;

            BackupUrlBox.Text = _settings.BackupUrl;
            BackupXp1Box.Text = _settings.BackupXPath1;
            BackupXp2Box.Text = _settings.BackupXPath2;
            BackupXp3Box.Text = _settings.BackupXPath3;
        }

        private async void TestMain_Click(object sender, RoutedEventArgs e)
        {
            TestStatus.Text = "测试主链接...";
            try
            {
                var result = await WebScraperService.FetchAsync(MainUrlBox.Text,
                    MainXp1Box.Text, MainXp2Box.Text, MainXp3Box.Text);
                if (string.IsNullOrWhiteSpace(result.Text1) && string.IsNullOrWhiteSpace(result.Text2) && string.IsNullOrWhiteSpace(result.Text3))
                    TestStatus.Text = "成功但内容为空";
                else
                    TestStatus.Text = "测试成功 ✓";
            }
            catch
            {
                TestStatus.Text = "测试失败 ✗";
            }
        }

        private async void TestBackup_Click(object sender, RoutedEventArgs e)
        {
            TestStatus.Text = "测试备用链接...";
            try
            {
                var result = await WebScraperService.FetchAsync(BackupUrlBox.Text,
                    BackupXp1Box.Text, BackupXp2Box.Text, BackupXp3Box.Text);
                if (string.IsNullOrWhiteSpace(result.Text1) && string.IsNullOrWhiteSpace(result.Text2) && string.IsNullOrWhiteSpace(result.Text3))
                    TestStatus.Text = "成功但内容为空";
                else
                    TestStatus.Text = "测试成功 ✓";
            }
            catch
            {
                TestStatus.Text = "测试失败 ✗";
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            _settings.MainUrl = MainUrlBox.Text;
            _settings.MainXPath1 = MainXp1Box.Text;
            _settings.MainXPath2 = MainXp2Box.Text;
            _settings.MainXPath3 = MainXp3Box.Text;
            _settings.BackupUrl = BackupUrlBox.Text;
            _settings.BackupXPath1 = BackupXp1Box.Text;
            _settings.BackupXPath2 = BackupXp2Box.Text;
            _settings.BackupXPath3 = BackupXp3Box.Text;
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