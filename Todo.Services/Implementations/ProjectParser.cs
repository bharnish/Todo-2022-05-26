using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Todo.Core;

namespace Todo.Services.Implementations
{
    public class ProjectParser : IProjectParser
    {
        public IEnumerable<string> Parse(string raw)
        {
            var regex = new Regex(Patterns.ProjectPattern);

            var contexts = regex.Matches(raw).Select(x => x.Value.Trim());

            return contexts;
        }
    }
}