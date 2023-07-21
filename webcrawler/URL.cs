// Ignore Spelling: webcrawler
using System;
using System.Collections.Generic;

namespace webcrawler
{
    internal class URL
    {
        private static int nextID = 0;
        public URL(int parentID, uint depth, int spiderID, string urlAddress)
        {
            this.URLID = nextID++;
            this.ParentID = parentID;
            this.Depth = depth;
            this.SpiderID = spiderID;
            this.URLAddress = urlAddress;
            this.FoundingDate = DateTime.Now;
            IsFailed = false;
        }
        public int URLID { get; set; }
        public int ParentID { get; set; }
        public uint Depth { get; set; }
        public int SpiderID { get; set; }
        public int CreatedURLCount { get; set; }
        public string URLAddress { get; set; }
        public DateTime FoundingDate { get; set; }
        public DateTime CrawlingDate { get; set; }
        public bool IsFailed { get; set; }
        public void Failed()
        {
            this.IsFailed = true;
        }
        public bool IsValid()
        {
            return Uri.TryCreate(this.URLAddress, UriKind.Absolute, out Uri result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
