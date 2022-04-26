using console_application.Data;
using console_application.Models;
using System;
using Xunit;
using Type = console_application.Models.Type;

namespace meeting_tests.MeetingTests
{
    public class TestCreatingMeeting
    {
        private readonly Database db;
        private readonly DbInMemory dbInMemory;

        public TestCreatingMeeting()
        {
            db = new Database();
            db.Meeting.Add( new Meeting(0, "Meeting", "1Person", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new()));
            
            db.Meeting.Add(
                new Meeting(1, "Meeting", "1Person", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new()));
            
            dbInMemory = new DbInMemory(db);
        }

        [Fact]
        public void WhenCreatingCorrectMeeting_ThenReturnShouldBeTrue()
        {
            Meeting exampleMeeting =
                new Meeting(2, "Meeting3", "1Person", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new());

            var response = dbInMemory.CreateMeeting(exampleMeeting);
            Assert.True(response.isSuccess);
        }

        [Fact]
        public void WhenCreatingMeetingNull_ThenReturnShouldBeFalse()
        {
            Meeting exampleMeeting = null;
            var response = dbInMemory.CreateMeeting(exampleMeeting);
            Assert.False(response.isSuccess);
        }

        [Fact]
        public void WhenCreatingCorrectMeeting_ThenItShouldBeAddedToDb()
        {
            Meeting exampleMeeting =
                new Meeting(2, "Meeting3", "1Person", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new());

            var response = dbInMemory.CreateMeeting(exampleMeeting);
            Database updatedDatabase = new Database(dbInMemory);

            Assert.True(updatedDatabase.Meeting.Exists(m => m == exampleMeeting));
        }

        [Fact]
        public void WhenCreatingMeetingNull_ThenItShouldNotBeAddedToDb()
        {
            Meeting exampleMeeting = null;

            int oldMeetingCount = db.Meeting.Count;

            var response = dbInMemory.CreateMeeting(exampleMeeting);
            Database updatedDatabase = new Database(dbInMemory);

            Assert.True(oldMeetingCount == updatedDatabase.Meeting.Count);
        }
    }
}
