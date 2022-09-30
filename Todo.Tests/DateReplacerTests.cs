using System;
using NUnit.Framework;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class DateReplacerTests
    {
        private IDateReplacer _dateReplacer;

        [SetUp]
        public void Setup()
        {
            _dateReplacer = new DateReplacer();
        }

        [Test]
        public void ReplaceDue()
        {
            var actual = _dateReplacer.ReplaceDue("due:2022-10-01", "2022-10-01", DateTime.Parse("2000-11-02"));
            Assert.That(actual, Is.EqualTo("due:2000-11-02"));

            actual = _dateReplacer.ReplaceDue("due:2022-10-01", DateTime.Parse("2022-10-01"), DateTime.Parse("2000-11-02"));
            Assert.That(actual, Is.EqualTo("due:2000-11-02"));
        }

        [Test]
        public void ReplaceThreshold()
        {
            var actual = _dateReplacer.ReplaceThreshold("t:2022-10-01", "2022-10-01", DateTime.Parse("2000-11-02"));
            Assert.That(actual, Is.EqualTo("t:2000-11-02"));

            actual = _dateReplacer.ReplaceThreshold("t:2022-10-01", DateTime.Parse("2022-10-01"), DateTime.Parse("2000-11-02"));
            Assert.That(actual, Is.EqualTo("t:2000-11-02"));
        }
    }
}