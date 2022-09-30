using Todo.Data;

namespace Todo.Services.Implementations
{
    public class Postponer : IPostponer
    {
        private readonly IDateParser _dateParser;
        private readonly IDateReplacer _dateReplacer;

        public Postponer(IDateParser dateParser, IDateReplacer dateReplacer)
        {
            _dateParser = dateParser;
            _dateReplacer = dateReplacer;
        }

        public void Postpone(DBRecord rec, int ndays)
        {
            var dueDate = _dateParser.ParseDueDate(rec.Data);
            if (dueDate == null) return;

            var newDueDate = dueDate.Value.AddDays(ndays);

            rec.Data = _dateReplacer.ReplaceDue(rec.Data, dueDate.Value, newDueDate);
        }

        public void PostponeThreshold(DBRecord rec, int ndays)
        {
            var thresholdDate = _dateParser.ParseThresholdDate(rec.Data);
            if (thresholdDate == null) return;

            var newThresholdDate = thresholdDate.Value.AddDays(ndays);

            rec.Data = _dateReplacer.ReplaceThreshold(rec.Data, thresholdDate.Value, newThresholdDate);
        }
    }
}