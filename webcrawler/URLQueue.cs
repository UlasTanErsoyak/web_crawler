using System;
using System.Collections.Generic;
using webcrawler.settings;
namespace webcrawler.queue
{
    class URLQueue
    {
        private Settings current_settings;
        private Queue<string> url_queue = new Queue<string>();
        private int total_crawled_urls= 0;
        private int desired_urls;
        public URLQueue()
        { 
            current_settings = new Settings();
            current_settings.load_settings();
            this.desired_urls = current_settings.max_url;
        }
        public bool EnqueueUrl(string url)
        {
            lock (url_queue)
            {
                if (total_crawled_urls < desired_urls)
                {
                    url_queue.Enqueue(url);
                    return true;
                }
                return false;
            }
        }
        public string DequeueUrl()
        {
            lock (url_queue)
            {
                if (url_queue.Count > 0)
                {
                    total_crawled_urls++;
                    return url_queue.Dequeue();
                }
                return null;
            }
        }
    }
}
