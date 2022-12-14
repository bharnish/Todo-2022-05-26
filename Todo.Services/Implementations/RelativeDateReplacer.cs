using System;
using System.Text.RegularExpressions;
using Todo.Core;
using Todo.Data;

namespace Todo.Services.Implementations
{
    public class RelativeDateReplacer : IScoped, IRelativeDateReplacer
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDateReplacer _dateReplacer;
        private readonly IDateParser _dateParser;

        public RelativeDateReplacer(IDateTimeProvider dateTimeProvider, IDateReplacer dateReplacer, IDateParser dateParser)
        {
            _dateTimeProvider = dateTimeProvider;
            _dateReplacer = dateReplacer;
            _dateParser = dateParser;
        }

        public DBRecord ReplaceRelativeDates(DBRecord record)
        {
            var raw = record.Data;

            var today = _dateTimeProvider.Today;
            raw = ReplaceText(raw, "today", today);
            raw = ReplaceText(raw, "tomorrow", today.AddDays(1));
            raw = ReplaceText(raw, "yesterday", today.AddDays(-1));

            raw = ReplaceDOW(raw, "monday", Next(DayOfWeek.Monday));
            raw = ReplaceDOW(raw, "tuesday", Next(DayOfWeek.Tuesday));
            raw = ReplaceDOW(raw, "wednesday", Next(DayOfWeek.Wednesday));
            raw = ReplaceDOW(raw, "thursday", Next(DayOfWeek.Thursday));
            raw = ReplaceDOW(raw, "friday", Next(DayOfWeek.Friday));
            raw = ReplaceDOW(raw, "saturday", Next(DayOfWeek.Saturday));
            raw = ReplaceDOW(raw, "sunday", Next(DayOfWeek.Sunday));
             
            raw = ReplaceDWMY(raw);

            raw = ReplaceParsable(raw);

            record.Data = raw;
            return record;

            DateTime Next(DayOfWeek dayOfWeek)
            {
                var days = dayOfWeek - today.DayOfWeek;
                if (days <= 0) days += 7;
                return today.AddDays(days);
            }

            string ReplaceDOW(string data, string text, DateTime date)
            {
                data = ReplaceText(data, text, date);
                return ReplaceText(data, text.Substring(0, 3), date);
            }

        }

        private string ReplaceParsable(string raw)
        {
            if (!_dateParser.ParseThresholdDate(raw).HasValue)
                raw = Replace(raw, "t:");

            if (!_dateParser.ParseDueDate(raw).HasValue)
                raw = Replace(raw, "due:");

            return raw;

            string Replace(string raw, string thresholdOrDue)
            {
                var regex = new Regex(thresholdOrDue + @"(?<date>[^\s]+)");
                var match = regex.Match(raw);
                if (!match.Success) return raw;

                var dateString = match.Groups["date"];

                if (!DateTime.TryParse(dateString.Value, out var date)) return raw;

                if (date < _dateTimeProvider.Today)
                    date = date.AddYears(1);

                return regex.Replace(raw, thresholdOrDue + date.ToString(Patterns.DateFormat));
            }
        }

        private string ReplaceText(string data, string text, DateTime date)
        {
            data = _dateReplacer.ReplaceDue(data, text, date);
            return _dateReplacer.ReplaceThreshold(data, text, date);
        }

        private string ReplaceDWMY(string raw)
        {
            const string pattern = Patterns.RelativeDatePattern;

            raw = Replace(raw, "t:");
            return Replace(raw, "due:");

            string Replace(string raw, string prefix)
            {
                var regex = new Regex(prefix + pattern);
                if (!regex.IsMatch(raw)) return raw;

                var groups = regex.Match(raw).Groups;

                var quantity = int.Parse(groups["quantity"].Value);
                var period = groups["period"].Value[0];

                var date = _dateTimeProvider.Today;

                date = period switch
                {
                    'd' => date.AddDays(quantity),
                    'w' => date.AddDays(quantity * 7),
                    'm' => date.AddMonths(quantity),
                    'y' => date.AddYears(quantity),
                    _ => throw new Exception("Invalid period"),
                };

                raw = regex.Replace(raw, prefix + date.ToString(Patterns.DateFormat));

                return raw;
            }
        }
    }
}
