using System;
using NUnit.Framework.Constraints;
using Todo.Services;

namespace Todo.Tests
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        public TestDateTimeProvider(string dateTime)
        {
            Now = DateTime.Parse(dateTime);
        }

        public TestDateTimeProvider() : this("2022-10-01")
        {
        }

        public DateTime Now { get; }
    }
}