// Ignore Spelling: webcrawler
using System.Collections.Generic;
namespace webcrawler
{
    internal class URLQueue : IURLCollection
    {
        private static Queue<URL> urlQueue = new Queue<URL>();
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
            return urlQueue.Count==0 ? null : urlQueue.Dequeue();
        }
    }
}
