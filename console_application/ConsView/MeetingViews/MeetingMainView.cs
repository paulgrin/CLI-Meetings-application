using console_application.Data;
using console_application.Parameters;
using System.Globalization;
using Type = console_application.Parameters.Type;

namespace console_application.ConsViews.MeetingViews
{
    public sealed class MeetingMainView : ConsView
    {
        public MeetingMainView(CLIContext context) : base(context) { 
            _commands = GenerateCommands();
        }

        public MeetingMainView(CLIContext context, string successMsg, string failureMsg) : base(context, successMsg, failureMsg) {
            _commands = GenerateCommands();
        }

        
        protected override string ViewName => "Welcome to Visma's internal meetings!";
        protected override string ViewDescription => "Navigate by typing commands available in brackets: Example type 1 and press Enter if you want to access [1]";


        protected override List<Command> GenerateCommands() {
            List<Command> commands = new();
            commands.Add(new CommCreateMeeting());
            commands.Add(new CommMeetingViewList());
            commands.Add(new CommQuit());
            
            return commands;
        }

        private class CommMeetingViewList : Command {
            public override string InvocationName => "2";
            public override string Description => "View all created meetings & Filter meetings by specific parameters";
            public override string[] Arguments => Array.Empty<string>();

            public override ConsView ExecuteCommand(CLIContext context, string[] args) {
                IEnumerable<Meeting> meetings;
                context.DbInMemory.GetMeetings(out meetings);

                return new MeetingsListView(context, meetings);
            }
        }

        private class CommCreateMeeting : Command { 
            public override string InvocationName => "1";
            public override string Description => "Create now new meeting";
            public override string[] Arguments => Array.Empty<string>();

            public override ConsView ExecuteCommand(CLIContext context, string[] args) {
                
                Meeting meeting = new Meeting();
                Console.Clear();

                Console.Write("What is your name?: ");
                meeting.ResponsiblePerson = Console.ReadLine();

                Console.Write("Please enter meeting name: ");
                meeting.Name = Console.ReadLine();

                Console.Write("Write the description of the meeting: ");
                meeting.Description = Console.ReadLine();

     // Category //
                Category? category = null;
                while (category == null) 
                {
      
                    string query = "In which category it belongs to? (";
                    foreach (var value in Enum.GetValues(typeof(Category)))
                    {
                        query += $"{value.ToString()}, ";
                    }
                    query = query[0..^2];
                    query += "): ";
                    Console.Write(query);
                    var output = Console.ReadLine();
                    Category cat;
                    if (Enum.TryParse<Category>(output, true, out cat))
                        category = cat;
                    else
                        Console.WriteLine("Typed incorrect, please use one of the following:\n" +
                            " CodeMonkey / Hub / Short / TeamBuilding");

                }

                meeting.Category = (Category)category;

     // Type //
                Type? type = null;
                while (type == null)
                {
                
                    string query = "In which type it belongs to? (";
                    foreach (var value in Enum.GetValues(typeof(Type)))
                    {
                        query += $"{value.ToString()}, ";
                    }
                    query = query[0..^2];
                    query += "): ";
                    Console.Write(query);
                    var output = Console.ReadLine();
                    Type typ;           
                    if (Enum.TryParse<Type>(output, true, out typ))
                        type = typ;
                    else
                        Console.WriteLine("Type was written incorrectly, try again");
                }

                meeting.Type = (Type)type;

     // Start Date //
                while (true) 
                { 
                
                    DateTime stDate;
                    Console.Write("Select when the meeting starts (Use this format \'yyyy-MM-dd HH:mm\'): ");
                    var output = Console.ReadLine();
                        if (!DateTime.TryParseExact(output, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out stDate))
                    {
                        Console.WriteLine("Incorrect format");
                        continue;
                    }
                        if (stDate <= DateTime.Now)
                    {
                        Console.WriteLine("Date provided has already happened!");
                        continue;
                    }

                    meeting.StartDate = stDate;
                    break;
                    
                }

     // End Date //
                while (true) 
                {
                
                    DateTime endDate;
                    Console.Write("Select when the meeting ends (Use this format: \'yyyy-MM-dd HH:mm\'): ");
                    var output = Console.ReadLine();
                        if (!DateTime.TryParseExact(output, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    {
                        Console.WriteLine("Incorrect format");
                        continue;
                    }
                        if (endDate <= DateTime.Now)
                    {
                        Console.WriteLine("Already happened!");
                        continue;
                    }
                    meeting.EndDate = endDate;
                    break;
                }

                meeting.Id = context.DbInMemory.IdForNewElement;
                meeting.Attendees = new();

                DbResponse res = context.DbInMemory.CreateMeeting(meeting);

                if (res.isSuccess)
                {
                    return new MeetingMainView(context, res.sucessMsg, "");
                }
                else
                {
                    return new MeetingMainView(context, "", res.errorMsg);
                }
                
            }

        }
    }
}
