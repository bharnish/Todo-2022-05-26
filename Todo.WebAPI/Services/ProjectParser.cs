using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class ProjectParser : IScoped
    {
        public IEnumerable<string> Parse(string raw)
        {
            var regex = new Regex(Patterns.ProjectPattern);

            var contexts = regex.Matches(raw).Select(x => x.Value.Trim());

            return contexts;
        }
    }
}