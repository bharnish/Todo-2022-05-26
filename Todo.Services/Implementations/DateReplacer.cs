using System;
using Todo.Core;

namespace Todo.Services.Implementations
{
    public class DateReplacer : IDateReplacer
    {
        public string ReplaceDue(string raw, DateTime oldDate, DateTime newDate) =>
            Replace(raw, "due:", oldDate, newDate);

        public string ReplaceThreshold(string raw, DateTime oldDate, DateTime newDate) =>
            Replace(raw, "t:", oldDate, newDate);

        public string ReplaceDue(string raw, string oldDate, DateTime newDate) =>
            Replace(raw, "due:", oldDate, newDate);

        public string ReplaceThreshold(string raw, string oldDate, DateTime newDate) =>
            Replace(raw, "t:", oldDate, newDate);

        private string Replace(string raw, string prefix, DateTime oldDate, DateTime newDate) => 
            Replace(raw, prefix, oldDate.ToString(Patterns.DateFormat), newDate);

        private string Replace(string raw, string prefix, string oldDate, DateTime newDate)
        {
            var newDateStr = newDate.ToString(Patterns.DateFormat);

            return raw.Replace(prefix + oldDate, prefix + newDateStr);
        }
    }
}