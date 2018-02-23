using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CacheBot.Tools
{
    public class BotCommandParser
    {
        public ExpandoObject Parse(string commandText)
        {
            var commandList = commandText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            var top = CommandFileParser.Parse();
            if (!top.Name.Equals(commandList.First(), StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var command = new List<KeyValuePair<string, string>>();
            foreach (var segment in commandList)
            {
                var nextCommand = top.Children.FirstOrDefault(c => c.Name.Equals(segment, StringComparison.InvariantCultureIgnoreCase));
                if (nextCommand != null)
                {
                    command.Add(new KeyValuePair<string, string>(nextCommand.CommandType.ToString(), nextCommand.Name));
                    top = nextCommand;
                }
                else
                {
                    nextCommand = top.Children.FirstOrDefault(c => c.CommandType == ParserCommandType.Variable);
                    if (nextCommand == null) continue;
                    command.Add(new KeyValuePair<string, string>(nextCommand.Name, segment));
                    top = nextCommand;
                }
            }

            var retCommand = new ExpandoObject() as IDictionary<string, object>;
            foreach (var keyValuePair in command)
            {
                retCommand.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return (ExpandoObject)retCommand;
        }
    }
}