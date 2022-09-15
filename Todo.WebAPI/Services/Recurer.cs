using System;
using System.Linq;
using System.Text.RegularExpressions;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class Recurer : IScoped
    {
        private readonly DateParser _dateParser;
        private readonly DateReplacer _dateReplacer;

        public Recurer(DateParser dateParser, DateReplacer dateReplacer)
        {
            _dateParser = dateParser;
            _dateReplacer = dateReplacer;
        }

        public DBRecord Recur(DBRecord rec)
        {
            var regex = new Regex(Patterns.RecurPattern);
            if (!regex.IsMatch(rec.Data)) return null;

            var raw = rec.Data;

            var recurTraits = ParseRecur(raw, regex);

            var dueDate = _dateParser.ParseDueDate(raw);
            if (dueDate != null)
            {
                var newDueDate = AdvanceDate(dueDate.Value, recurTraits);
                raw = _dateReplacer.ReplaceDue(raw, dueDate.Value, newDueDate);
            }

            var threshold = _dateParser.ParseThresholdDate(raw);
            if (threshold != null)
            {
                var newThreshold = AdvanceDate(threshold.Value, recurTraits);
                raw = _dateReplacer.ReplaceThreshold(raw, threshold.Value, newThreshold);
            }

            return new DBRecord {Data = raw};
        }

        private DateTime AdvanceDate(DateTime date, (bool strict, int num, char period) recurTraits)
        {
            if (!recurTraits.strict)
                date = DateTime.Today;

            switch (recurTraits.period)
            {
                case 'd':
                    date = date.AddDays(recurTraits.num);
                    break;
                case 'w':
                    date = date.AddDays(recurTraits.num * 7);
                    break;
                case 'm': 
                    date = date.AddMonths(recurTraits.num);
                    break;
                case 'y': 
                    date = date.AddYears(recurTraits.num);
                    break;
            }

            return date;

        }

        private (bool strict, int num, char period) ParseRecur(string raw, Regex regex)
        {
            var recur = regex.Match(raw).Groups["date"].Value.Trim() ?? "";

            var strict = recur.StartsWith("+");
            if (strict)
                recur = recur.Substring(1);

            var period = recur.Last();
            recur = recur.Substring(0, recur.Length - 1);

            var num = int.Parse(recur);

            return (strict, num, period);
        }
    }
}