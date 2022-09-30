using System.Collections.Generic;
using Todo.Data;

namespace Todo.Services
{
    public interface ITodoMutator
    {
        IEnumerable<DBRecord> MarkCompleted(DBRecord rec, bool isCompleted);
        void Postpone(DBRecord rec, int ndays);
        void PostponeThreshold(DBRecord rec, int ndays);
        DBRecord ReplaceRelativeDates(DBRecord record);
    }
}