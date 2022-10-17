using NUnit.Framework;
using Todo.Data;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class PostponerTests
    {
        private IPostponer _postponer;

        [SetUp]
        public void Setup()
        {

            _postponer = new Postponer(new DateParser(), new DateReplacer());
        }

        [Test]
        public void PostponeTest()
        {
            var rec = new DBRecord
            {
                Data = "due:2022-10-01",
            };

            _postponer.Postpone(rec, 1);

            Assert.That(rec.Data, Is.EqualTo("due:2022-10-02"));
        }

        [Test]
        public void ThresholdTest()
        {
            var rec = new DBRecord
            {
                Data = "t:2022-10-01",
            };

            _postponer.PostponeThreshold(rec, 1);

            Assert.That(rec.Data, Is.EqualTo("t:2022-10-02"));
        }
    }
}