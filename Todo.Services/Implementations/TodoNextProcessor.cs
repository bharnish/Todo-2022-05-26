using System.Text.RegularExpressions;
using Todo.Core;
using Todo.Data;

namespace Todo.Services.Implementations
{
    public class TodoNextProcessor : ITodoNextProcessor
    {
        public DBRecord TodoNext(DBRecord rec)
        {
            var regex = new Regex(Patterns.TodoNextPattern);
            if (!regex.IsMatch(rec.Data)) return null;

            var value = regex.Match(rec.Data).Groups["item"].Value;

            return new DBRecord { Data = value };
        }
    }
}
