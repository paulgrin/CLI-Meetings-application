namespace console_application.ConsViews
{
    public sealed class CommQuit : Command
    {
        public override string InvocationName => "x";

        public override string[] Arguments => Array.Empty<string>();

        public override string Description => "Quit the program";

        public override ConsView ExecuteCommand(CLIContext context, string[] args)
        {
            return null;
        }
    }
}
