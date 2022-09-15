using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class TodoNextProcessor : IScoped
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
