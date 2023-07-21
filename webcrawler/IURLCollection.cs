// Ignore Spelling: webcrawler
namespace webcrawler
{
    internal interface IURLCollection
    {
        void Add(URL url);
        URL Pop();
        bool IsEmpty();

    }
}
