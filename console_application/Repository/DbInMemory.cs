using console_application.Parameters;

namespace console_application.Data
{
    public class DbInMemory : IMeeting
    {
        public DbInMemory(Database db)
        {
            _meetings = new List<Meeting>(db.Meeting);
        }

        public int IdForNewElement
        {
            get
            {
                var lastMeeting = _meetings.LastOrDefault();
                if (lastMeeting == null)
                    return 1;

                return lastMeeting.Id + 1;
            }
        }

        public DbResponse CreateMeeting(Meeting meeting)
        {
            if (meeting == null)
                return new DbResponse(false, "Meeting provided is null", "");
            _meetings.Add(meeting);
            return new DbResponse(true, "", "Meeting added successfully");
        }

        public DbResponse DeleteMeeting(int id)
        {
            var delObj = _meetings.Where(m => m.Id == id).SingleOrDefault();
            if (delObj == null)
                return new DbResponse(false, "No such meeting found", "");

            _meetings.Remove(delObj);
            return new DbResponse(true, "", "Meeting deleted successfully");            
        }

        public DbResponse GetMeetings(out IEnumerable<Meeting> meetings)
        {
            meetings = new List<Meeting>(_meetings);

            return new DbResponse(true, "", "Meetings retrieved successfully");
        }

        public DbResponse AddAttendee(int meetingId, string person)
        {

            var meeting = _meetings.Where(m => m.Id == meetingId).SingleOrDefault();
            if (meeting == null)
                throw new Exception("Meeting not found. Possibly the data is out of sync.");

            // Check if added person is not already in here:
            
            // 1. As an organizer
            if (meeting.ResponsiblePerson == person)
                return new DbResponse(false, "Person trying to be added already exists as an organizer", "");
            // 2. As an attendee
            if (meeting.Attendees.Exists(p => p == person))
                return new DbResponse(false, "Person trying to be added already exists as an attendee", "");

            meeting.Attendees.Add(person);
            return new DbResponse(true, "", "Attendee added successfully");
        }

        public DbResponse RemoveAttendee(int meetingId, string person)
        {
            var meeting = _meetings.Where(m => m.Id == meetingId).SingleOrDefault();
            if (meeting == null)
                throw new Exception("Meeting not found. Possibly the data is out of sync.");

            // Check if removed person:
            // 1. Is not an organizer
            if (meeting.ResponsiblePerson == person)
                return new DbResponse(false, "You can not remove a meeting organizer", "");
            // 2. Exists in the attendee list
            if (!meeting.Attendees.Exists(p => p == person))
                return new DbResponse(false, "No such person found", "");

            meeting.Attendees.Remove(person);
            return new DbResponse(true, "", "Attendee removed successfully");
        }

        private List<Meeting> _meetings;
    }
}
