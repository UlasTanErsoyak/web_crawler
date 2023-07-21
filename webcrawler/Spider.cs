using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using webcrawler.Logs;
//implement settings as end condition
//implement vertical and horizontal traversal
//methodize everything
//clean up the code
//database integration
namespace webcrawler
{
    internal class Spider
    {
        private static HashSet<string> crawledUrls = new HashSet<string>();
        private readonly HttpClient httpClient= new HttpClient();
        Logger logger = new Logger();
        private readonly int numberOfThreads;
        public Spider(int numberOfThreads)
        {
            this.numberOfThreads = numberOfThreads;
        }
        public async Task<bool> Crawl(MainWindow mainWindow,IURLCollection urlCollection)
        {
            while (!urlCollection.IsEmpty())
            {
                URL url = urlCollection.Pop();
                if (!crawledUrls.Contains(url.URLAddress))
                {
                    try
                    {
                        if (url.IsValid())
                        {
                            IEnumerable<string> foundUrls = await GetUrlsAsync(url);
                            url.CreatedURLCount = foundUrls.Count();
                            url.CrawlingDate = DateTime.Now;
                            mainWindow.Dispatcher.Invoke(() =>
                            {
                                mainWindow.crawled_url_listbox.Items.Add(url.ToString());
                            });
                            crawledUrls.Add(url.URLAddress);
                            foreach (var node in foundUrls)
                            {
                                URL newURL = new URL(url.ParentID, url.Depth + 1, Thread.CurrentThread.ManagedThreadId, node);
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
            return true;
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

