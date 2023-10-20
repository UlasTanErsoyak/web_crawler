using System;
using System.Collections.Concurrent;
using System.Threading;
using webcrawler.Logs;
namespace webcrawler
{
    internal class URLStack : IURLCollection
    {
        private static ConcurrentStack<URL> urlStack = new ConcurrentStack<URL>();
        private Logger logger = new Logger();
        private SemaphoreSlim semaphore = new SemaphoreSlim(10);
        public void Push(URL url)
        {
            urlStack.Push(url);
        }
        public bool IsEmpty()
        {
            return urlStack.Count == 0;
        }

        public URL TryPop()
        {
            try
            {
                semaphore.Wait();
                return urlStack.TryPop(out URL url) ? url : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }
        public int Count()
        {
            return urlStack.Count;
        }
    }
}
