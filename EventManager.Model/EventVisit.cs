using SQLite;

namespace EventManager.Model
{
    public class EventVisit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Indexed(Name = "EventVisitCombineIDX", Order = 1, Unique = true)]
        public int IdEvent { get; set; }

        [NotNull]
        [Indexed(Name = "EventVisitCombineIDX", Order = 2, Unique = true)]
        public int IdPlayer { get; set; }
    }
}
