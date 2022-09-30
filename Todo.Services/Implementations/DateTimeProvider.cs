using System;
using Todo.Core;

namespace Todo.Services.Implementations
{
    public class DateTimeProvider : IDateTimeProvider, IScoped
    {
        public DateTime Now => DateTime.UtcNow.AddHours(-4);
    }
}