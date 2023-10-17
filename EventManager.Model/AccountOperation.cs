using SQLite;
using System;

namespace EventManager.Model
{
    public class AccountOperation
    {
        public enum enAccountOperationType { Incoming = 0, Outgoing = 1 }

        public enum enAccountOperationKind { PayPerVisit = 0, AbonimentPayment = 1, RentPayment = 2 }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime Date { get; set; }

        [NotNull]
        public int ResultBalance { get; set; }

        [NotNull]
        public int Amount { get; set; }

        [NotNull]
        public enAccountOperationType Type { get; set; }

        [NotNull]
        public enAccountOperationKind Kind { get; set; }
    }
}
