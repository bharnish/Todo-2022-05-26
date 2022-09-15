using System;
using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class DateParser : IScoped
    {
        public DateTime? ParseDueDate(string raw)
        {
            var regex = new Regex(Patterns.DueDatePattern);
            var dueDate =  regex.Match(raw).Groups["date"].Value.Trim() ?? "";

            if (string.IsNullOrEmpty(dueDate))
                return null;

            return DateTime.Parse(dueDate);
        }

        public DateTime? ParseThresholdDate(string raw)
        {
            var regex = new Regex(Patterns.ThresholdDatePattern);
            var threshold = regex.Match(raw).Groups["date"].Value.Trim() ?? "";

            if (string.IsNullOrEmpty(threshold))
                return null;

            return DateTime.Parse(threshold);
        }

        public DateTime? ParseCompletedDate(string raw)
        {
            if (!raw.StartsWith("x ")) return null;

            var dateString = raw.Substring(2, 10);

            if (DateTime.TryParse(dateString, out var date))
                return date;
            
            return null;
        }
    }
}