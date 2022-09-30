using System.Collections.Generic;

namespace Todo.Services
{
    public interface IContextParser
    {
        IEnumerable<string> Parse(string raw);
    }
}