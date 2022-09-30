using Todo.Data;

namespace Todo.Services
{
    public interface ITodoParser
    {
        Data.Todo Parse(DBRecord record);
    }
}