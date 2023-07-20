using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using webcrawler.Url;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace webcrawler.spider_factory
{
    class Spider
    {
        private int maxUrlsToCrawl;
        private Queue<string> urlsToCrawl = new Queue<string>();
        private HashSet<string> visitedUrls = new HashSet<string>();
        private HttpClient httpClient = new HttpClient();
        public Spider(int spider_id,int crawled_url_count=0,int found_url_count=0,int failed_url_count=0)
        {
            this.spider_id = spider_id;
            this.crawled_url_count = crawled_url_count;
            this.found_url_count = found_url_count;
            this.failed_url_count = failed_url_count;
        }
        public int spider_id { get; set; }
        public int crawled_url_count { get; set; }
        public int found_url_count { get; set; }
        public int failed_url_count { get; set; }

        public async Task crawl(string url)
        {
            //todo
        }
    }
}
