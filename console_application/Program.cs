namespace console_application
{
    public class Program
    {
        public const string JsonFileName = @"../database.json";
        public const string JsonFileName2 = @"../database2.json";

        public const string DayTimeFormat = "yyyy-MM-dd";

        private static void Main()
        {
            Console.WriteLine(JsonFileName);
            var ctx = new CLIContext();
            ctx.Run();
        }
    }
}

