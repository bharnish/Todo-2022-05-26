using Todo.Core;
using Todo.Data;

namespace Todo.Services.Implementations
{
    public class TodoParser  : IScoped, ITodoParser
    {
        private readonly IDateParser _dateParser;
        private readonly IContextParser _contextParser;
        private readonly IProjectParser _projectParser;
        private readonly IPriorityParser _priorityParser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TodoParser(IDateParser dateParser, IContextParser contextParser, IProjectParser projectParser, IPriorityParser priorityParser, IDateTimeProvider dateTimeProvider)
        {
            _dateParser = dateParser;
            _contextParser = contextParser;
            _projectParser = projectParser;
            _priorityParser = priorityParser;
            _dateTimeProvider = dateTimeProvider;
        }

        public Data.Todo Parse(DBRecord record)
        {
            var raw = record.Data;

            var todo = new Data.Todo
            {
                Id = record.Id,
                Raw = raw,
                DueDate = _dateParser.ParseDueDate(raw),
                ThresholdDate = _dateParser.ParseThresholdDate(raw),
                CompletedDate = _dateParser.ParseCompletedDate(raw),
                Contexts = _contextParser.Parse(raw),
                Projects = _projectParser.Parse(raw),
                Priority = _priorityParser.ParsePriority(raw),
                Today = _dateTimeProvider.Today,
            };

            return todo;
        }
    }
}