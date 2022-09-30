using System;

namespace Todo.Services
{
    public interface IDateReplacer
    {
        string ReplaceDue(string raw, DateTime oldDate, DateTime newDate);
        string ReplaceDue(string raw, string oldDate, DateTime newDate);
        string ReplaceThreshold(string raw, DateTime oldDate, DateTime newDate);
        string ReplaceThreshold(string raw, string oldDate, DateTime newDate);
    }
}