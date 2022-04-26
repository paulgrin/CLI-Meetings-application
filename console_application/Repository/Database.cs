using console_application.Parameters;

namespace console_application.Data
{
    public class Database
    {
        public Database()
        {
            Meeting = new List<Meeting>();
        }
        public Database(DbInMemory dbInMemory)
        {
            IEnumerable<Meeting> meetings;
            dbInMemory.GetMeetings(out meetings);
            Meeting = new List<Meeting>(meetings);
        }
        public List<Meeting> Meeting { get; set; }
    }
}
