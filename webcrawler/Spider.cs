using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace webcrawler
{
    internal class Spider
    {
        private static List<URL> urls = new List<URL>();
        private static HashSet<string> crawledUrls = new HashSet<string>();
        private readonly HttpClient httpClient= new HttpClient();
        public async Task<string> Crawl(URLQueue queue,MainWindow mainWindow)
        {
            while (!queue.IsEmpty())
            {
                URL url = queue.Pop();
                if (!crawledUrls.Contains(url.URLAddress))
                {
                    var html = await httpClient.GetStringAsync(url.URLAddress);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var anchorTags = doc.DocumentNode.SelectNodes("//a[@href]")
                        .Where(node => !node.Descendants("img").Any()) // Filter out anchor tags containing images
                        .Select(node => node.GetAttributeValue("href", ""))
                        .Where(href => Uri.TryCreate(href, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                        .ToArray();
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.crawled_url_listbox.Items.Add(url.URLAddress);
                    });
                    crawledUrls.Add(url.URLAddress);
                    foreach (var node in anchorTags)
                    {
                        URL newurl = new URL(url.ParentID, 0, Thread.CurrentThread.ManagedThreadId, node);
                        urls.Add(newurl);
                        queue.Add(newurl);
                    }
                }
                
            }
            return "x";
        }
    }

}

