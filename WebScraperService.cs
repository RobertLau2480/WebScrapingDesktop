using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScrapingDesktop
{
    public class ScrapeResult
    {
        public string Text1 { get; set; } = "";
        public string Text2 { get; set; } = "";
        public string Text3 { get; set; } = "";
    }

    public static class WebScraperService
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        /// <summary>
        /// 使用指定 URL 或本地文件路径以及三个 XPath 抓取文字 (异步)
        /// 支持 http://、https:// 链接和本地文件路径（如 C:\page.html 或 file:///C:/page.html）
        /// </summary>
        public static async Task<ScrapeResult> FetchAsync(string url, string xpath1, string xpath2, string xpath3)
        {
            var result = new ScrapeResult();
            string html;

            try
            {
                if (IsLocalFile(url))
                {
                    // 本地文件：读取文件内容
                    html = await ReadLocalFileAsync(url);
                }
                else
                {
                    // 网络请求
                    html = await FetchHttpAsync(url);
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                result.Text1 = GetInnerText(doc, xpath1);
                result.Text2 = GetInnerText(doc, xpath2);
                result.Text3 = GetInnerText(doc, xpath3);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"抓取失败: {ex.Message}");
                throw;
            }

            return result;
        }

        /// <summary>
        /// 判断给定的 URL 是否为本地文件路径
        /// （不以 http:// 或 https:// 开头）
        /// </summary>
        private static bool IsLocalFile(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;
            return !url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                   !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取本地 HTML 文件内容（异步）
        /// 自动处理 file:// 协议前缀，并转换为完整路径
        /// </summary>
        private static async Task<string> ReadLocalFileAsync(string filePath)
        {
            // 移除可能的 file:// 前缀
            if (filePath.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring(7).TrimStart('/');
            }

            // 获取绝对路径
            string fullPath = Path.GetFullPath(filePath);
            return await File.ReadAllTextAsync(fullPath);
        }

        /// <summary>
        /// 发起 HTTP 请求，获取响应文本（异步）
        /// </summary>
        private static async Task<string> FetchHttpAsync(string url)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            HttpResponseMessage response = await _httpClient.GetAsync(url, cts.Token);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 根据 XPath 提取节点内部文本，并对 HTML 实体进行解码
        /// </summary>
        private static string GetInnerText(HtmlDocument doc, string xpath)
        {
            try
            {
                var node = doc.DocumentNode.SelectSingleNode(xpath);
                string rawText = node?.InnerText?.Trim() ?? "";
                // 二次解码，将 &#x27;、&amp;、&lt; 等实体转为正常字符
                return WebUtility.HtmlDecode(rawText);
            }
            catch
            {
                return "";
            }
        }
    }
}