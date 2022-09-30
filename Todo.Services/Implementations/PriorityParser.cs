using System.Text.RegularExpressions;
using Todo.Core;

namespace Todo.Services.Implementations
{
    public class PriorityParser : IPriorityParser
    {
        public string ParsePriority(string raw)
        {
            var regex = new Regex(Patterns.PriorityPattern);
            var priority = regex.Match(raw).Groups["priority"].Value.Trim() ?? "";

            if (string.IsNullOrEmpty(priority))
                return null;

            return priority;
        }
    }
}