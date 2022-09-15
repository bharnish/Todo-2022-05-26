using System;

namespace Todo.WebAPI.Services
{
    public interface IDateTimeProvider
    {
        public DateTime Today { get; }
        public DateTime Now { get; }
    }
}