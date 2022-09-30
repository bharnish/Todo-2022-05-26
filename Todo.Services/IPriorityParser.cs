namespace Todo.Services
{
    public interface IPriorityParser
    {
        string ParsePriority(string raw);
    }
}