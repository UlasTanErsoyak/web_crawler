// Ignore Spelling: webcrawler
//8 out of 10
namespace webcrawler
{
    internal interface IURLCollection
    {
        void Add(URL url);
        URL Pop();
        bool IsEmpty();
        int Count();
    }
}
