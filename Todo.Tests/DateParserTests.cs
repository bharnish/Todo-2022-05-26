using System;
using NUnit.Framework;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class DateParserTests
    {
        private IDateParser _dateParser;

        [SetUp]
        public void Setup()
        {
            _dateParser = new DateParser();
        }

        [Test]
        public void NoDates()
        {
            var raw = "some text";

            var date = _dateParser.ParseDueDate(raw);
            Assert.That(date, Is.Null);

            date = _dateParser.ParseThresholdDate(raw);
            Assert.That(date, Is.Null);

            date = _dateParser.ParseCompletedDate(raw);
            Assert.That(date, Is.Null);
        }

        [Test]
        public void DueDate()
        {
            var actual = _dateParser.ParseDueDate("due:2022-10-01");

            Assert.That(actual.HasValue);

            Assert.That(actual, Is.EqualTo(DateTime.Parse("2022-10-01")));
        }
        
        [Test]
        public void ThresholdDate()
        {
            var actual = _dateParser.ParseThresholdDate("t:2022-10-01");

            Assert.That(actual.HasValue);

            Assert.That(actual, Is.EqualTo(DateTime.Parse("2022-10-01")));
        }
        
        [Test]
        public void CompleteDate()
        {
            var actual = _dateParser.ParseCompletedDate("x 2022-10-01");

            Assert.That(actual.HasValue);

            Assert.That(actual, Is.EqualTo(DateTime.Parse("2022-10-01")));
        }
    }
}