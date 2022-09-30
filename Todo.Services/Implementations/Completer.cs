using Todo.Core;
using Todo.Data;

namespace Todo.Services.Implementations
{
    public class Completer : IScoped, ICompleter
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public Completer(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void Complete(DBRecord rec) => rec.Data = $"x {_dateTimeProvider.Today.ToString(Patterns.DateFormat)} " + rec.Data;
        public void Uncomplete(DBRecord rec) => rec.Data = rec.Data.Substring($"x {Patterns.DateFormat} ".Length);
    }
}