using Todo.Data;

namespace Todo.Services
{
    public interface IPostponer
    {
        void Postpone(DBRecord rec, int ndays);
        void PostponeThreshold(DBRecord rec, int ndays);
    }
}