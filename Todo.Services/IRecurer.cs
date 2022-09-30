using Todo.Data;

namespace Todo.Services
{
    public interface IRecurer
    {
        DBRecord Recur(DBRecord rec);
    }
}