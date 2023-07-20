// Ignore Spelling: webcrawler

using System;
namespace webcrawler.Url
{
    public class URL
    {
        public URL(int id,int depth,int spider_id,int parent_id,int status_code,
            int byte_length,int child_count,string content, string? url_address)
        {
            this.id = id;
            this.depth = depth;
            this.spider_id = spider_id;
            this.parent_id = parent_id;
            this.status_code = status_code;
            this.byte_length = byte_length;
            this.child_count = child_count;
            this.content = content;
            this.url_address = url_address;

        }
        public int id { get; set; }
        public int depth { get; set; }
        public int spider_id { get; set; }
        public string? url_address { get; set; }
        public int parent_id { get; set; }
        public int status_code { get; set; }
        public int byte_length { get; set; }
        public int child_count { get; set; }
        public string content { get; set; }
        public  bool is_valid()
        {
            try
            {
                return Uri.TryCreate(this.url_address, UriKind.Absolute, out Uri resultUri)
                    && (resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
        public override string ToString()
        {
            return $"url id: {id}, depth: {depth}, spider id: {spider_id}, url: {url_address}," +
                   $" parent id: {parent_id}";
        }
        public override bool Equals(object obj)
        {
            if (obj is URL otherUrl)
            {
                return this.id == otherUrl.id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
