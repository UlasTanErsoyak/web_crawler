// Ignore Spelling: webcrawler
//8 out of 10
namespace webcrawler
{
    internal interface IURLCollection
    {
        void Push(URL url);
        URL TryPop();
        bool IsEmpty();
        int Count();
    }
}
