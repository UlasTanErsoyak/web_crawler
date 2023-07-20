// Ignore Spelling: webcrawler

using System;
namespace webcrawler
{
    class URL
    {
        public int id { get; set; }
        public int depth { get; set; }
        public int spider_id { get; set; }
        public string? url_address { get; set; }
        public int parent_id { get; set; }
        public int child_count { get; set; }
        public int status_code { get; set; }
        public int byte_length { get; set; }
        public  bool is_valid()
        {
            return Uri.TryCreate(this.url_address, UriKind.Absolute, out Uri resultUri)
                && (resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);
        }
        public override string ToString()
        {
            return $"url id: {id}, depth: {depth}, spider id: {spider_id}, url: {url_address}," +
                   $" parent id: {parent_id}, child count: {child_count}";
        }
        public override bool Equals(object obj)
        {
            if (obj is URL otherUrl)
            {
                return this.id == otherUrl.id;
            }
            return false;
        }
    }
}
