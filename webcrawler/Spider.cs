using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using webcrawler.Logs;
using WebCrawler.Settings;
//methodize everything
//clean up the code
//database integration
namespace webcrawler
{
    internal class Spider
    {
        private static HashSet<string> crawledUrls = new HashSet<string>();
        private readonly HttpClient httpClient= new HttpClient();
        private Logger logger = new Logger();
        private readonly int numberOfTasks;
        private WebCrawlerSettings settings = new WebCrawlerSettings();
        private int maxDepth = -1;
        private object lockObject = new object();
        public Spider(int numberOfTasks)
        {
            this.numberOfTasks = numberOfTasks;
            settings.LoadSettings();
        }
        public async Task<bool> Crawl(MainWindow mainWindow, IURLCollection urlCollection)
        {
            var tasks = new List<Task>();
            var spiderIds = Enumerable.Range(0, numberOfTasks).ToList();

            for (int i = 0; i < numberOfTasks; i++)
            {
                int spiderId = spiderIds[i];
                tasks.Add(ProcessUrls(mainWindow, urlCollection, spiderId));
            }

            await Task.WhenAll(tasks);
            return true;
        }
        private async Task ProcessUrls(MainWindow mainWindow, IURLCollection urlCollection, int spiderId)
        {
            while (!urlCollection.IsEmpty() && crawledUrls.Count < settings.MaxUrl && maxDepth <= settings.MaxDepth)
            {
                URL url;
                lock (lockObject)
                {
                    if (urlCollection.IsEmpty())
                        return;

                    url = urlCollection.Pop();
                }

                if (!crawledUrls.Contains(url.URLAddress))
                {
                    try
                    {
                        url.SpiderID = spiderId;
                        if (url.IsValid())
                        {
                            IEnumerable<string> foundUrls = await GetUrlsAsync(url);

                            url.CreatedURLCount = foundUrls.Count();
                            url.CrawlingDate = DateTime.Now;
                            crawledUrls.Add(url.URLAddress);
                            logger.Info(url.SpiderID.ToString());

                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                mainWindow.crawled_url_listbox.Items.Add(url.ToString());
                                mainWindow.url_count_label.Content = crawledUrls.Count;
                            });
                            if (url.Depth + 1 > maxDepth)
                            {
                                maxDepth = url.Depth + 1;
                            }

                            foreach (var node in foundUrls)
                            {
                                URL newURL = new URL(url.URLID, url.Depth + 1, -1, node);
                                urlCollection.Add(newURL);
                            }
                        }
                        else
                        {
                            logger.Warning($"{url.URLAddress} was not valid.");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        url.IsFailed = true;
                    }
                }
            }
        }

        private async Task<IEnumerable<string>> GetUrlsAsync(URL url)
        {
            try
            {
                var html = await httpClient.GetStringAsync(url.URLAddress);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var anchorTags = doc.DocumentNode.SelectNodes("//a[@href]")
                    .Where(node => !node.Descendants("img").Any())
                    .Select(node => node.GetAttributeValue("href", ""))
                    .Where(href => Uri.TryCreate(href, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)).ToArray();
                return anchorTags;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                url.IsFailed = true;
                return Enumerable.Empty<string>();
            }
        }
    }
}

