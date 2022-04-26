namespace console_application.ConsViews
{
    public abstract class ConsView
    {
        public ConsView(CLIContext context)
        {
            _context = context;
        }

        public ConsView(CLIContext context, string successMsg, string failureMsg)
        {
            _context = context;
            _successMsg = successMsg;
            _failureMsg = failureMsg;
        }

        public string SuccessMsg { get { return _successMsg; } }
        public string FailureMsg { get { return _failureMsg; } }

        public void ClearMessages()
        {
            _successMsg = String.Empty;
            _failureMsg = String.Empty;
        }

        public string GetContents()
        {
            if (!string.IsNullOrEmpty(_contentBuffer))
                return _contentBuffer;

            _contentBuffer = "";

            _contentBuffer += $"{ViewName}\n\n";
            if (!String.IsNullOrEmpty(ViewDescription))
            {
                _contentBuffer += $"{ViewDescription}\n\n";
            }
            foreach (Command command in _commands)
            {
                _contentBuffer += $"[{command.InvocationName}] ";
                
                foreach (var arg in command.Arguments)
                {
                    _contentBuffer += $"|{arg}|";
                }
                if (command.Arguments.Length > 0)
                {
                    _contentBuffer += " ";
                }
                _contentBuffer += $"- {command.Description}\n";
            }

            if (_commands.Count > 0)
                _contentBuffer += "\n";

            return _contentBuffer;
        }

        public bool CommandExists(string userInput)
        {
            return _commands.Exists(c => c.InvocationName == userInput);
        }

        public ConsView CallCommand(string commandName, string[] args)
        {
            if (!CommandExists(commandName))
            {
                _failureMsg = "Command not found";
                return this;
            }

            var command = _commands.Where(c => c.InvocationName == commandName).Single();

            return command.ExecuteCommand(_context, args);
        }

        
        protected List<Command> _commands = new();
        protected abstract string ViewName { get; }
        protected abstract string ViewDescription { get; }
        protected abstract List<Command> GenerateCommands();

        
        private CLIContext _context;
        private string _contentBuffer = String.Empty;
        private string _successMsg = String.Empty;
        private string _failureMsg = String.Empty;
    }
}
