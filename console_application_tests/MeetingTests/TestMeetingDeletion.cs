using console_application.Data;
using console_application.Models;
using System;
using Xunit;
using Type = console_application.Models.Type;

namespace meeting_tests.MeetingTests
{
    public class TestMeetingDeletion
    {
        private readonly Database db;
        private readonly DbInMemory dbInMemory;

        public TestMeetingDeletion()
        {
            db = new Database();
            db.Meeting.Add(
                new Meeting(0, "Meeting", "Person1", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new()));
            
            db.Meeting.Add(
                new Meeting(1, "Meeting", "Person1", "No description", Category.CodeMonkey, Type.InPerson, DateTime.Now, DateTime.Now + new TimeSpan(2, 0, 0),
                new()));
            
            dbInMemory = new DbInMemory(db);
        }

        [Fact]
        public void WhenDeletingMeetingWithRightId_ThenTheResponseShouldBeTrue()
        {
            int idOfFirstElement = db.Meeting[0].Id;

            var response = dbInMemory.DeleteMeeting(idOfFirstElement);
            Assert.True(response.isSuccess);
        }

        [Fact]
        public void WhenDeletingMeetingWithRightId_ThenTheMeetingShouldBeDeleted()
        {
            int idOfFirstElement = db.Meeting[0].Id;

            var response = dbInMemory.DeleteMeeting(idOfFirstElement);
            Database updatedDatabase = new Database(dbInMemory);

            Assert.False(updatedDatabase.Meeting.Exists(m => m.Id == idOfFirstElement));
        }

        [Fact]
        public void WhenDeletingMeetingWithWrongId_ThenResponseShouldBeError()
        {
            int idOfFirstElement = 321312;

            var response = dbInMemory.DeleteMeeting(idOfFirstElement);
            Assert.False(response.isSuccess);
        }
    }
}
