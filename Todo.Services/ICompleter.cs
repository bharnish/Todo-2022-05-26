using Todo.Data;

namespace Todo.Services
{
    public interface ICompleter
    {
        void Complete(DBRecord rec);
        void Uncomplete(DBRecord rec);
    }
}