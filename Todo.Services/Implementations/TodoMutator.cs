using System.Collections.Generic;
using Todo.Core;
using Todo.Data;

namespace Todo.Services.Implementations
{
    public class TodoMutator : IScoped, ITodoMutator
    {
        private readonly ICompleter _completer;
        private readonly IRecurer _recurer;
        private readonly ITodoNextProcessor _todoNextProcessor;
        private readonly IPostponer _postponer;
        private readonly IRelativeDateReplacer _relativeDateReplacer;

        public TodoMutator(ICompleter completer, IRecurer recurer, ITodoNextProcessor todoNextProcessor, IPostponer postponer, IRelativeDateReplacer relativeDateReplacer)
        {
            _completer = completer;
            _recurer = recurer;
            _todoNextProcessor = todoNextProcessor;
            _postponer = postponer;
            _relativeDateReplacer = relativeDateReplacer;
        }

        public IEnumerable<DBRecord> MarkCompleted(DBRecord rec, bool isCompleted)
        {
            if (isCompleted)
            {
                var recur = _recurer.Recur(rec);
                var next = _todoNextProcessor.TodoNext(rec);
                _completer.Complete(rec);
                if (recur != null)
                    yield return recur;
                if (next != null)
                    yield return next;
            }
            else
            {
                _completer.Uncomplete(rec);
            }
        }

        public void Postpone(DBRecord rec, int ndays) => _postponer.Postpone(rec, ndays);

        public void PostponeThreshold(DBRecord rec, int ndays) => _postponer.PostponeThreshold(rec, ndays);

        public DBRecord ReplaceRelativeDates(DBRecord record) => _relativeDateReplacer.ReplaceRelativeDates(record);
    }
}