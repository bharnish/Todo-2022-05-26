using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class PriorityParser : IScoped
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