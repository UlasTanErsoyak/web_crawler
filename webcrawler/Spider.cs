// Ignore Spelling: webcrawler
using HtmlAgilityPack;
using System.Net.Http;
using System;
using webcrawler.Logs;
using WebCrawler.Settings;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Controls;

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
        private int crawledUrlsCount = 0;
        private DateTime startTime;
        Label crawling_rate_label;
        public Spider(int spiderId, bool type,List<string> workload,string tableName,Label crawl_label)
        {
            this.tableName = tableName;
            this.SpiderId = spiderId;
            settings = new WebCrawlerSettings();
            crawling_rate_label = crawl_label;
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
            while (this.CrawledUrls < settings.MaxUrl && currentDepth <= settings.MaxDepth)
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
                        this.CrawledUrls++;
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
                        crawledUrlsCount++;
                        double crawlingRate = GetCrawlingRate();
                        crawling_rate_label.Dispatcher.Invoke(() =>
                        {
                            crawling_rate_label.Content = $"{crawlingRate}";
                        });
                        await Task.Delay(settings.Delay);
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
        public double GetCrawlingRate()
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            double elapsedSeconds = elapsed.TotalSeconds;
            double crawlingRate = crawledUrlsCount / elapsedSeconds;
            return crawlingRate;
        }
    }
}

