using EventManager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.DataAccess
{
    public interface IDBProvider
    {
        IEnumerable<News> GetNews(int? count);
        SQLiteRepository<TTEntity> GetRepository<TTEntity>() where TTEntity : class, new();
        void AddPaymentToEvent(Person member, Event @event, int amount);
        Task RemovePaymentAsync(Person member, Event @event);
    }
}