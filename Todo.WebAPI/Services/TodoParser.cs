using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class TodoParser  : IScoped
    {
        private readonly DateParser _dateParser;
        private readonly ContextParser _contextParser;
        private readonly ProjectParser _projectParser;
        private readonly PriorityParser _priorityParser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TodoParser(DateParser dateParser, ContextParser contextParser, ProjectParser projectParser, PriorityParser priorityParser, IDateTimeProvider dateTimeProvider)
        {
            _dateParser = dateParser;
            _contextParser = contextParser;
            _projectParser = projectParser;
            _priorityParser = priorityParser;
            _dateTimeProvider = dateTimeProvider;
        }

        public Domain.Todo Parse(DBRecord record)
        {
            var raw = record.Data;

            var todo = new Domain.Todo
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