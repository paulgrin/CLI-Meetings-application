using console_application.Data;
using console_application.Parameters;
using Type = console_application.Parameters.Type;

namespace console_application.ConsViews.MeetingViews
{
    public sealed class MeetingInfoView : ConsView
    {
        public MeetingInfoView(CLIContext context, Meeting meeting) : base(context)
        {
            _meeting = meeting;
            _commands = GenerateCommands();
        }

        public MeetingInfoView(CLIContext context, Meeting meeting, string successMsg, string failureMsg) : base(context, successMsg, failureMsg)
        {
            _meeting = meeting;
            _commands = GenerateCommands();
        }

        protected override string ViewName => $"Meeting {_meeting.Name}";

        
        protected override string ViewDescription {
            get {
                string reply = "";
                       reply += "Description: " + _meeting.Description + "\n";
                       reply += "Created by: " + _meeting.ResponsiblePerson + "\n";
                       reply += "Category: " + Enum.GetName(typeof(Category), _meeting.Category) + "\n";
                       reply += "Type: " + Enum.GetName(typeof(Type), _meeting.Type) + "\n";
                       reply += "Starts: " + _meeting.StartDate.ToString("yyyy-MM-dd HH:mm") + "\n";
                       reply += "Ends: " + _meeting.EndDate.ToString("yyyy-MM-dd HH:mm") + "\n";
                       reply += "Attendees: ";
                
                foreach (var attendee in _meeting.Attendees)
                {
                    reply += $"{attendee}, ";
                }
                
                reply = reply[0..^2];
                
                return reply;
            }
        }

        protected override List<Command> GenerateCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new CommRemoveMeeting(_meeting));
            commands.Add(new CommAddAttendee(_meeting));
            commands.Add(new CommRemoveAttendee(_meeting));
            commands.Add(new CommReturnToList());
            commands.Add(new CommQuit());

            return commands;
        }

        private Meeting _meeting;

        private class CommRemoveMeeting : Command
        {
            public CommRemoveMeeting(Meeting meeting)
            {
                _meeting = meeting;
            }

            public override string InvocationName => "DEL";
            public override string[] Arguments => Array.Empty<string>();
            public override string Description => "Delete a meeting fully";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                Console.WriteLine("Are you sure you want to delete the meeting (TYPE: yes or no");
                string c = Console.ReadLine();
                
                    if (c != "yes")
                {
                    return new MeetingInfoView(context, _meeting, "", "Delete operation aborted");
                }

                DbResponse ans = context.DbInMemory.DeleteMeeting(_meeting.Id);

                IEnumerable<Meeting> meetings;
                context.DbInMemory.GetMeetings(out meetings);

                if (ans.isSuccess)
                    return new MeetingsListView(context, meetings, ans.sucessMsg, "");
                else
                    return new MeetingsListView(context, meetings, "", ans.errorMsg);
            }
            private Meeting _meeting;
        }

        
        private class CommAddAttendee : Command
        {
            public CommAddAttendee(Meeting meeting)
            {
                _meeting = meeting;
            }
            
            public override string InvocationName => "ADD";
            public override string[] Arguments => new string[] { "Attendee name" };
            public override string Description => "Added to this meeting";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length == 0)
                    return new MeetingInfoView(context, _meeting, "", "No argument for person found");
                
                string name = args[0];
                if (args.Length > 1)
                    name = string.Join(" ", args);

                DbResponse res = context.DbInMemory.AddAttendee(_meeting.Id, name);

                if (res.isSuccess)
            {
                    IEnumerable<Meeting> meetings;
                    context.DbInMemory.GetMeetings(out meetings);
                    IEnumerable<Meeting> overlappingMeetings = GetOverlappingMeetings(meetings, _meeting, name);
                    return new MeetingInfoView(context, _meeting, res.sucessMsg, OverlappingMeetingsText(overlappingMeetings, name));
            }
                else
                {
                    return new MeetingInfoView(context, _meeting, "", res.errorMsg);
                }
            }

            
            private static string OverlappingMeetingsText(IEnumerable<Meeting> overlappingMeetings, string attendeeName)
            {
                if (overlappingMeetings.Any())
                {
                    var meetings = overlappingMeetings.Select(m => m.Name).ToArray();
                    if (meetings == null)
                        return "";

                    string message = "";
                    if (meetings.Count() == 1)   
                        message = $"{attendeeName} overlap with meeting {meetings[0]}";
                    else
                    {
                        message = $"{attendeeName} overlap with meetings {String.Join(", ", meetings)}";
                        message = message[0..^2];
                    }
                    return message;
                }
                else
                {
                    return "";
                }
            }

            private static IEnumerable<Meeting> GetOverlappingMeetings(IEnumerable<Meeting> allMeetings, Meeting analyzedMeeting, string attendee)
            {
                List<Meeting> overlappers = new List<Meeting>();
                foreach (Meeting other in allMeetings)
                {
                    if (other.Equals(analyzedMeeting))
                        continue;

                    if (!other.Attendees.Contains(attendee))
                        continue;

                    if ((analyzedMeeting.StartDate >= other.StartDate && analyzedMeeting.StartDate <= other.EndDate) 
                        ||
                        (analyzedMeeting.EndDate >= other.StartDate && analyzedMeeting.EndDate <= other.EndDate))
                        overlappers.Add(other);
                }
                return overlappers;
            }
            private Meeting _meeting;
        }

        
        private class CommRemoveAttendee : Command
        {
            public CommRemoveAttendee(Meeting meeting)
            {
                _meeting = meeting;
            }
            public override string InvocationName => "R";
            public override string[] Arguments => new string[] { "Attendee name" };
            public override string Description => "Remove an attendee from this meeting";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length == 0)
                    return new MeetingInfoView(context, _meeting, "", "No argument found");
                
                string name = args[0];
                if (args.Length > 1)
                    name = string.Join(" ", args);

                Console.WriteLine(name);
                
                DbResponse res = context.DbInMemory.RemoveAttendee(_meeting.Id, name);
                return new MeetingInfoView(context, _meeting, res.sucessMsg, res.errorMsg);
            }
            private Meeting _meeting;
        }

        
        private class CommReturnToList : Command
        {
            public override string InvocationName => "r";
            public override string[] Arguments => Array.Empty<string>();
            public override string Description => "Return to the list view";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                IEnumerable<Meeting> meetings;
                context.DbInMemory.GetMeetings(out meetings);

                return new MeetingsListView(context, meetings);
            }
        }
    }
}
