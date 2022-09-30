using Todo.Data;

namespace Todo.Services
{
    public interface IRelativeDateReplacer
    {
        DBRecord ReplaceRelativeDates(DBRecord record);
    }
}