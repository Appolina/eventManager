using EventManager.Model;

using SQLite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.DataAccess
{
    public class EventRepository : IDBProvider, INewsProcessor, INewsProvider
    {
        private const string TOKEN_VALUE_NAME = "Token";
        private const string GROUP_ID_VALUE_NAME = "GroupId";
        private const string SIGN_SUFFIX_VALUE_NAME = "SignSuffix";
        private const string DEFAULT_SIGN_SUFFIX_VALUE = " в 21:00. 1.5 часа. 2 минуты от м. площадь Мужества. Стоимость 150р. Непокорённых 10. Кто желает, записывайтесь!";
        public SQLiteConnection con;
        private SQLiteAsyncConnection asyncConnection;
        public event Action OnNewsUpdate;


        public EventRepository(string dbname)
        {
            this.con = new SQLiteConnection(dbname, true);
            this.asyncConnection = new SQLiteAsyncConnection(dbname, true);
        }

        public void CreateTable()
        {
            //con.DropTable<Event>();
            //con.DropTable<News>();
            //con.DropTable<EventVisit>();
            //con.DropTable<EventPayment>();
            //con.DropTable<AppServiceValue>();

            //con.CreateTable<Event>();
            con.CreateTable<EventVisit>();
            con.CreateTable<News>();
            con.CreateTable<EventPayment>();
            con.CreateTable<AppServiceValue>();

            if (string.IsNullOrEmpty(getValueFor(SIGN_SUFFIX_VALUE_NAME)))
                this.SaveSettingValue(SIGN_SUFFIX_VALUE_NAME, DEFAULT_SIGN_SUFFIX_VALUE);

        }

        internal DateTime GetLatestNewsDate()
        {
            var latestNews = con.Table<News>().OrderByDescending(n => n.NewsDate).FirstOrDefault();

            if (latestNews == null)
                return DateTime.MinValue;
            else
                return latestNews.NewsDate;
        }


        public IEnumerable<int> GetVisitorsIdForEvent(Event @event)
        {
            var visitsForEvent = con.Table<EventVisit>().Where(e => e.IdEvent == @event.Id);
            foreach (var visit in visitsForEvent)
            {
                yield return visit.IdPlayer;
            }
        }


        public IEnumerable<News> GetNews(int? count)
        {
            if (count.HasValue)
            {
                return con.Table<News>().OrderByDescending(n => n.NewsDate).Take(count.Value);
            }
            else
            {
                return con.Table<News>().OrderByDescending(n => n.NewsDate);
            }
        }

        internal string GetSignSuffixValue()
        {
            return getValueFor(SIGN_SUFFIX_VALUE_NAME);
        }

        internal string GetGroupIdValue()
        {
            return getValueFor(GROUP_ID_VALUE_NAME);
        }

        public string GetTokenValue()
        {
            return getValueFor(TOKEN_VALUE_NAME);
        }

        private string getValueFor(string settingName)
        {
            var tokenAppSettingValue = con.Table<AppServiceValue>().Where(v => v.Name == settingName);

            if (tokenAppSettingValue.Count() == 0)
                return null;
            else if (tokenAppSettingValue.Count() > 1)
                throw new Exception($"There are more then one row for value {settingName}");
            else
                return tokenAppSettingValue.First().Value;
        }

        private void insertNews(News news)
        {
            if (this.OnNewsUpdate != null)
                this.OnNewsUpdate();

            news.Id = con.Insert(news);
        }

        public void AddNewsProvider(INewsMaker newsProvider)
        {
            newsProvider.OnNewsHappen += NewsProvider_OnNewsHappen;
        }

        private void NewsProvider_OnNewsHappen(INewsMaker sender, News news)
        {
            this.insertNews(news);


        }

        internal void SaveSettingValue(string settingName, string settingValue)
        {
            var settings = con.Table<AppServiceValue>().Where(v => v.Name == settingName);

            if (settings.Count() == 1)
            {
                var tokenAppValue = settings.First();
                tokenAppValue.Value = settingValue;
                con.Update(tokenAppValue);
            }
            else
            {
                con.Insert(new AppServiceValue() { Name = settingName, Value = settingValue });
            }
        }

        public SQLiteRepository<TTEntity> GetRepository<TTEntity>() where TTEntity : class, new()
        {
            return new SQLiteRepository<TTEntity>(this.asyncConnection);
        }

        public void AddPaymentToEvent(Person member, Event @event, int amount)
        {
            con.Insert(new EventPayment() { IdEvent = @event.Id, IdPayer = member.Id, Amount = amount });
        }

        internal void SaveAccessToken(string accessToken)
        {
            var settingName = TOKEN_VALUE_NAME;
            var settingValue = accessToken;

            this.SaveSettingValue(settingName, settingValue);

        }



        public async Task RemovePaymentAsync(Person member, Event @event)
        {
            var rep = this.GetRepository<EventPayment>();

            var objectToDelete = await rep.GetWhereAsync(p => p.IdEvent == @event.Id && p.IdPayer == member.Id);
            await rep.DeleteAsync(objectToDelete.First());
        }


    }
}
