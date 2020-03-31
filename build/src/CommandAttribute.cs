using System;

namespace Azxc
{
    class CommandAttribute : Attribute
    {
        public string commandName { get; set; }
        public bool forceTrim { get; set; }
        public bool forceLower { get; set; }

        public CommandAttribute(string commandName, bool trim = true, bool lower = true)
        {
            this.commandName = commandName;
            this.forceTrim = trim;
            this.forceLower = lower;
        }

        public string Command()
        {
            string command = commandName;
            if (forceTrim)
                command = command.Trim();
            if (forceLower)
                command = command.ToLower();
            return command;
        }
    }
}
