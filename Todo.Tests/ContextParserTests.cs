using System.Collections.Generic;
using NUnit.Framework;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class ContextParserTests
    {
        private IContextParser _contextParser;

        [SetUp]
        public void Setup()
        {
            _contextParser = new ContextParser();
        }

        [TestCase("@c1 some text +p1 @c2 @c3", ExpectedResult = new[]{ "@c1", "@c2", "@c3" })]
        [TestCase("@c1 some text +p1 @c2\" @c3", ExpectedResult = new[]{ "@c1", "@c2", "@c3" })]
        [TestCase("@c1 some text +p1 user@example.com @c2 @c3", ExpectedResult = new[]{ "@c1", "@c2", "@c3" })]
        public IEnumerable<string> Parse_Works(string raw)
        {
            var contexts = _contextParser.Parse(raw);

            return contexts;
        }
    }
}