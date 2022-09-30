using System;

namespace Todo.Services
{
    public interface IDateTimeProvider
    {
        public DateTime Today => Now.Date;
        public DateTime Now { get; }
    }
}