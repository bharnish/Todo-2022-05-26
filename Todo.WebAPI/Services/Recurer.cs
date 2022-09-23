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

            return recurTraits.period switch
            {
                'd' => date.AddDays(recurTraits.num),
                'w' => date.AddDays(recurTraits.num * 7),
                'm' => date.AddMonths(recurTraits.num),
                'y' => date.AddYears(recurTraits.num),
                _ => date,
            };
        }

        private (bool strict, int num, char period) ParseRecur(string raw, Regex regex)
        {
            var g = regex.Match(raw).Groups;

            var strict = g["strict"].Value == "+";
            var quantity = int.Parse(g["quantity"].Value);
            var period = g["period"].Value[0];

            return (strict, quantity, period);
        }
    }
}