namespace console_application.ConsViews
{
    public abstract class Command
    {
        public abstract string InvocationName { get; }
        public abstract string[] Arguments { get; }
        public abstract string Description { get; }

        public abstract ConsView ExecuteCommand(CLIContext context, string[] args);
    }
}
