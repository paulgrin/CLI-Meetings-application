using console_application.Parameters;
using System.Globalization;
using Type = console_application.Parameters.Type;

namespace console_application.ConsViews.MeetingViews
{
    public sealed class MeetingsListView : ConsView
    {
        public MeetingsListView(CLIContext context, IEnumerable<Meeting> meetings) : base(context)
        {
            _meetings = new(meetings);
            _commands = GenerateCommands();
        }

        public MeetingsListView(CLIContext context, IEnumerable<Meeting> meetings, string successMsg, string failureMsg) : base(context, successMsg, failureMsg)
        {
            _meetings = new(meetings);
            _commands = GenerateCommands();
        }

        protected override string ViewName => "----------------------------------------- MEETINGS LIST -----------------------------------------";

        protected override string ViewDescription => "Select requested command from [ ] to continue by following with filter parameters";

        protected override List<Command> GenerateCommands()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new CommQuit());
            commands.Add(new CommClearFilters());
            commands.Add(new CommReturnToStart());
            commands.Add(new CommFilterDescription(_meetings));
            commands.Add(new CommFilterResponsiblePerson(_meetings));
            commands.Add(new CommFilterCategory(_meetings));
            commands.Add(new CommFilterType(_meetings));
            commands.Add(new CommFilterDates(_meetings));
            commands.Add(new CommFilterAttendees(_meetings));
            
            foreach (var meeting in _meetings)
            {
                commands.Add(new ViewMeetingCommand(meeting));
            }
            return commands;
        }

        private List<Meeting> _meetings;


        private class ViewMeetingCommand : Command
        {
            public ViewMeetingCommand(Meeting meeting)
            {
                _meeting = meeting;
            }

            public override string InvocationName => $"2{_meeting.Id}";
            public override string[] Arguments => Array.Empty<string>();
            public override string Description => $"Select meeting \'{_meeting.Name}\' created by {_meeting.ResponsiblePerson} (Description: {_meeting.Description})";
           
            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                return new MeetingInfoView(context, _meeting);
            }

            private Meeting _meeting;
        }

        private class CommReturnToStart : Command
        {
            public override string InvocationName => "R";
            public override string[] Arguments => Array.Empty<string>();
            public override string Description => "Return\n";
            
            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                return new MeetingMainView(context);
            }
        }

        private class CommClearFilters : Command
        {
            public override string InvocationName => "CL";
            public override string[] Arguments => Array.Empty<string>();
            public override string Description => "Clear filters";
            
            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                IEnumerable<Meeting> meetings;
                context.DbInMemory.GetMeetings(out meetings);

                return new MeetingsListView(context, meetings);
            }
        }

        private class CommFilterDescription : Command
        {
            public CommFilterDescription(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            
            public override string InvocationName => "DC";
            public override string[] Arguments => new string[] { "Description" };
            public override string Description => "Filter meetings by description";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                string substring = string.Join(" ", args);
                _meetings = _meetings.Where(m => m.Description.Contains(substring)).ToList();
                return new MeetingsListView(context, _meetings, "Applied filter by description", "");
            }

            private List<Meeting> _meetings;
        }

        private class CommFilterResponsiblePerson : Command
        {
            public CommFilterResponsiblePerson(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            
            public override string InvocationName => "RP";
            public override string[] Arguments => new string[] { "Responsible person name" };
            public override string Description => "Filter meetings by meeting creator";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                string substring = string.Join(" ", args);
                _meetings = _meetings.Where(m => m.ResponsiblePerson == substring).ToList();
                return new MeetingsListView(context, _meetings, "Applied filter of filtering by creator's name", "");
            }

            private List<Meeting> _meetings;
        }

        private class CommFilterCategory : Command
        {
            public CommFilterCategory(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            
            public override string InvocationName => "FC";
            public override string[] Arguments => new string[] { "Category name" };
            public override string Description
            {
                get
                {
                    string query = "Filter by Category (";
                    foreach (var value in Enum.GetValues(typeof(Category)))
                    {
                        query += $"{value.ToString()}, ";
                    }
                    query = query[0..^2];
                    query += "): ";
                    return query;
                }
            }

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length != 1)
                    return new MeetingsListView(context, _meetings, "", "Incorrect argument length");
                Category category;
                if (!Enum.TryParse<Category>(args[0], true, out category))
                    return new MeetingsListView(context, _meetings, "", "Category not found");

                _meetings = _meetings.Where(m => m.Category == category).ToList();

                return new MeetingsListView(context, _meetings, "Applied filter to categories", "");
            }

            private List<Meeting> _meetings;
        }

        private class CommFilterType : Command
        {
            public CommFilterType(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            public override string InvocationName => "FT";
            public override string[] Arguments => new string[] { "Type name" };
            public override string Description
            {
                get
                {
                    string query = "Filter by Type (";
                    foreach (var value in Enum.GetValues(typeof(Type)))
                    {
                        query += $"{value.ToString()}, ";
                    }
                    query = query[0..^2];
                    query += "): ";
                    return query;
                }
            }

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length != 1)
                    return new MeetingsListView(context, _meetings, "", "Incorrect argument length");
                Type type;
                if (!Enum.TryParse<Type>(args[0], true, out type))
                    return new MeetingsListView(context, _meetings, "", "Category not found");

                _meetings = _meetings.Where(m => m.Type == type).ToList();

                return new MeetingsListView(context, _meetings, "Applied filter", "");
            }

            private List<Meeting> _meetings;
        }

        private class CommFilterDates : Command
        {
            public CommFilterDates(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            public override string InvocationName => "FD";

            public override string[] Arguments => new string[] { $"Start Date ({Program.DayTimeFormat})", $"End Date ({Program.DayTimeFormat})" };

            public override string Description => "Filter meetings between date ranges";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length != 2)
                    return new MeetingsListView(context, _meetings, "", "Incorrect argument length");

                DateTime stDate, endDate;
                if (!DateTime.TryParseExact(args[0], Program.DayTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out stDate))
                    return new MeetingsListView(context, _meetings, "", "Incorrect date format for start date provided");
                if (!DateTime.TryParseExact(args[1], Program.DayTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    return new MeetingsListView(context, _meetings, "", "Incorrect date format for end date provided");
                if (stDate >= endDate)
                    return new MeetingsListView(context, _meetings, "", "Start date cannot be later than end date");

                _meetings = _meetings.Where(m => m.StartDate >= stDate && m.EndDate <= endDate).ToList();

                return new MeetingsListView(context, _meetings, "Applied filter with dates", "");
            }

            private List<Meeting> _meetings;
        }

        private class CommFilterAttendees : Command
        {
            public CommFilterAttendees(List<Meeting> meetings)
            {
                _meetings = new(meetings);
            }
            public override string InvocationName => "FN";
            public override string[] Arguments => new string[] { "Attendee number" };
            public override string Description => "Filter by number of minimal attendee amount\n";

            public override ConsView ExecuteCommand(CLIContext context, string[] args)
            {
                if (args.Length != 1)
                    return new MeetingsListView(context, _meetings, "", "Incorrect argument length");

                int count;
                if (!int.TryParse(args[0], out count))
                    return new MeetingsListView(context, _meetings, "", "Provided argument is not a number");

                _meetings = _meetings.Where(m => m.Attendees.Count >= count).ToList();

                return new MeetingsListView(context, _meetings, "Applied filter with attendee count", "");
            }
            
            private List<Meeting> _meetings;
        }
    }
}
