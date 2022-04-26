using console_application.Parameters;

namespace console_application.Data
{
    internal interface IMeeting
    {
        public DbResponse CreateMeeting(Meeting meeting);
        public DbResponse DeleteMeeting(int id);
        public DbResponse AddAttendee(int meetingId, string person);
        public DbResponse RemoveAttendee(int meetingId, string person);
        public DbResponse GetMeetings(out IEnumerable<Meeting> meetings);
    }
}
