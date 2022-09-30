using Todo.Data;

namespace Todo.Services
{
    public interface ITodoNextProcessor
    {
        DBRecord TodoNext(DBRecord rec);
    }
}