// Ignore Spelling: webcrawler
using HtmlAgilityPack;
using System.Net.Http;
using System;
using webcrawler.Logs;
using WebCrawler.Settings;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace webcrawler
{
    internal class Spider
    {
        WebCrawlerSettings settings;
        private int currentDepth = 0;
        private readonly HttpClient httpClient = new HttpClient();
        Logger logger = new Logger();
        private readonly string tableName;
        private IURLCollection urlCollection;
        Database database = new Database();
        public Spider(int spiderId, bool type,List<string> workload,string tableName)
        {
            this.tableName = tableName;
            this.SpiderId = spiderId;
            settings = new WebCrawlerSettings();
            if (type)
            {
                urlCollection = new URLStack();
                AddWorkload(workload);
            }
            else
            {
                urlCollection = new URLQueue();
                AddWorkload(workload);
            }
            settings.LoadSettings();
        }
        public void AddWorkload(List<string> workload)
        {
            foreach(string url in workload)
            {
                URL Url = new URL(0, 1, this.SpiderId, url);
                Url.FoundingDate = DateTime.Now;
                urlCollection.Push(Url);
            }
        }
        int CrawledUrls { get; set; }
        int SpiderId { get; set; }
        public async Task Crawl(MainWindow mainWindow)
        {
            while (CrawledUrls < settings.MaxUrl && currentDepth <= settings.MaxDepth)
            {
                URL url = urlCollection.TryPop();
                if (url == null)
                {
                    break;
                }
                if (url.IsValid())
                {
                    try
                    {
                        var html = await httpClient.GetStringAsync(url.URLAddress);
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var foundUrls = doc.DocumentNode.SelectNodes("//a[@href]")
                            .Where(node => !node.Descendants("img").Any())
                            .Select(node => node.GetAttributeValue("href", ""))
                            .Where(href => Uri.TryCreate(href, UriKind.Absolute, out Uri uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)).ToArray();
                        url.CreatedURLCount = foundUrls.Length;
                        url.CrawlingDate = DateTime.Now;
                        url.SpiderID = this.SpiderId;
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.crawled_url_listbox.Items.Add(url.ToString());
                        });
                        database.SaveURL(this.tableName, url);
                        foreach (var x in foundUrls)
                        {
                            URL newURL = new URL(url.URLID, url.Depth + 1, -1, x);
                            newURL.FoundingDate = DateTime.Now;
                            urlCollection.Push(newURL);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        url.IsFailed = true;
                        url.CrawlingDate = DateTime.Now;
                        database.SaveURL(this.tableName, url);
                    }
                }
                else
                {
                    url.IsFailed = true;
                    url.CrawlingDate = DateTime.Now;
                    logger.Warning($"{url.URLAddress} was not valid.");
                    database.SaveURL(this.tableName, url);
                }
            }
        }
    }
}

