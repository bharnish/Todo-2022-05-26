using NUnit.Framework;
using Todo.Data;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class CompleterTests
    {
        private ICompleter _completer;
        private IDateTimeProvider _dateTimeProvider;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new TestDateTimeProvider();
            _completer = new Completer(_dateTimeProvider);
        }

        [Test]
        public void Complete_And_Uncomplete_work()
        {
            var original = "(A) Some text @c1 +p1";
            
            var record = new DBRecord
            {
                Data = original,
            };

            _completer.Complete(record);

            Assert.That(record.Data, Is.EqualTo($"x {_dateTimeProvider.Today:yyyy-MM-dd} {original}"));

            _completer.Uncomplete(record);
            
            Assert.That(record.Data, Is.EqualTo(original));
        }
    }
}