using EventManager.Model;

namespace EventManager.DataAccess
{
    internal interface INewsProcessor
    {
        void AddNewsProvider(INewsMaker newsMaker);
    }
}