using System.Collections.Generic;
using NUnit.Framework;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class ProjectParserTests
    {
        private IProjectParser _contextParser;

        [SetUp]
        public void Setup()
        {
            _contextParser = new ProjectParser();
        }

        [TestCase("@c1 some text +p1 +p2 +p3", ExpectedResult = new[]{ "+p1", "+p2", "+p3" })]
        [TestCase("@c1 some text +p1 +p2\" +p3", ExpectedResult = new[]{ "+p1", "+p2", "+p3" })]
        [TestCase("+p1 some text +p2 +p3", ExpectedResult = new[]{ "+p1", "+p2", "+p3" })]
        public IEnumerable<string> Parse_Works(string raw)
        {
            var contexts = _contextParser.Parse(raw);

            return contexts;
        }
    }
}