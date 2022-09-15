using System;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class DateTimeProvider : IDateTimeProvider, IScoped
    {
        public DateTime Today => Now.Date;
        public DateTime Now => DateTime.UtcNow.AddHours(-4);
    }
}