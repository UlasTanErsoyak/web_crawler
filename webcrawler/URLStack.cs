// Ignore Spelling: webcrawler
//7 out of 10
using System;
using System.Collections.Concurrent;
using webcrawler.Logs;
namespace webcrawler
{
    internal class URLStack : IURLCollection
    {
        private static ConcurrentStack<URL> urlStack = new ConcurrentStack<URL>();
        private Logger logger = new Logger();
        public void Add(URL url)
        {
            urlStack.Push(url);
            logger.Info("Successfully added an url to the stack.");
        }
        public bool IsEmpty()
        {
            return urlStack.Count == 0;
        }
        public URL Pop()
        {
            try
            {
                logger.Info("Successfully popped an url from the stack.");
                return urlStack.TryPop(out URL url) ? url : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        public int Count()
        {
            return urlStack.Count;
        }
    }
}
