using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class TodoMutator : IScoped
    {
        private readonly Recurer _recurer;
        private readonly TodoNextProcessor _todoNextProcessor;
        private readonly Postponer _postponer;
        private readonly DateReplacer _dateReplacer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly RelativeDateReplacer _relativeDateReplacer;

        public TodoMutator(Recurer recurer, TodoNextProcessor todoNextProcessor, Postponer postponer, DateReplacer dateReplacer, IDateTimeProvider dateTimeProvider, RelativeDateReplacer relativeDateReplacer)
        {
            _recurer = recurer;
            _todoNextProcessor = todoNextProcessor;
            _postponer = postponer;
            _dateReplacer = dateReplacer;
            _dateTimeProvider = dateTimeProvider;
            _relativeDateReplacer = relativeDateReplacer;
        }

        public IEnumerable<DBRecord> MarkCompleted(DBRecord rec, bool isCompleted)
        {
            if (isCompleted)
            {
                var recur = _recurer.Recur(rec);
                var next = _todoNextProcessor.TodoNext(rec);
                rec.Data = Complete(rec);
                if (recur != null)
                    yield return recur;
                if (next != null)
                    yield return next;
            }
            else
            {
                rec.Data = rec.Data.Substring(13);
            }
        }

        private string Complete(DBRecord rec) => $"x {_dateTimeProvider.Today.ToString(Patterns.DateFormat)} {rec.Data}";

        public void Postpone(DBRecord rec, int ndays) => _postponer.Postpone(rec, ndays);

        public void PostponeThreshold(DBRecord rec, int ndays) => _postponer.PostponeThreshold(rec, ndays);

        public DBRecord ReplaceRelativeDates(DBRecord record) => _relativeDateReplacer.ReplaceRelativeDates(record);
    }
}