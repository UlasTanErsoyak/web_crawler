// Ignore Spelling: webcrawler
//9 out of 10
using System;
using System.Collections.Concurrent;
using webcrawler.Logs;
namespace webcrawler
{
    internal class URLQueue : IURLCollection
    {
        private static ConcurrentQueue<URL> urlQueue = new ConcurrentQueue<URL>();
        Logger logger = new Logger();
        public void Add(URL url)
        {
            urlQueue.Enqueue(url);
        }
        public bool IsEmpty()
        {
            return urlQueue.Count==0;
        }
        public URL Pop()
        {
            try
            {
                return urlQueue.TryDequeue(out URL url) ? url : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        public int Count()
        {
            return urlQueue.Count;
        }
    }
}
