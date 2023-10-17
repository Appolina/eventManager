using SQLite;

namespace EventManager.Model
{
    public class EventPayment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Indexed(Name = "EventPaymentCombineIDX", Order = 1, Unique = true)]
        public int IdEvent { get; set; }

        [NotNull]
        [Indexed(Name = "EventPaymentCombineIDX", Order = 2, Unique = true)]
        public int IdPayer { get; set; }
        
        public int Amount { get; set; }

        [NotNull]
        public int IsAccountingOperation { get; set; }
    }
}