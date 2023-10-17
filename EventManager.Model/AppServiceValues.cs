using SQLite;

namespace EventManager.Model
{
    public class AppServiceValue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Unique]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
