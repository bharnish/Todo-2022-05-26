using System;

namespace Todo.Services
{
    public interface IDateParser
    {
        DateTime? ParseDueDate(string raw);
        DateTime? ParseThresholdDate(string raw);
        DateTime? ParseCompletedDate(string raw);
    }
}