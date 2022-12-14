using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Todo.Core;

namespace Todo.Services.Implementations
{
    public class ContextParser : IContextParser
    {
        public IEnumerable<string> Parse(string raw)
        {
            var regex = new Regex(Patterns.ContextPattern);

            var contexts = regex.Matches(raw).Select(x => x.Value.Trim());

            return contexts;
        }
    }
}