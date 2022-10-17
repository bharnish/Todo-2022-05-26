using NUnit.Framework;
using Todo.Services;
using Todo.Services.Implementations;

namespace Todo.Tests
{
    public class PriorityParserTests
    {
        private IPriorityParser _priorityParser;

        [SetUp]
        public void Setup()
        {
            _priorityParser = new PriorityParser();
        }

        [TestCase("(A) ABC", ExpectedResult = "(A)")]
        [TestCase("(Z) ABC", ExpectedResult = "(Z)")]
        [TestCase(" (Z) ABC", ExpectedResult = null)]
        [TestCase("ABC", ExpectedResult = null)]
        public string Test1(string input)
        {
            return _priorityParser.ParsePriority(input);
        }
    }
}