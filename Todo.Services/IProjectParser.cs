using System.Collections.Generic;

namespace Todo.Services
{
    public interface IProjectParser
    {
        IEnumerable<string> Parse(string raw);
    }
}