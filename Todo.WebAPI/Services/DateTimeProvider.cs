using System;
using Todo.WebAPI.Domain;

namespace Todo.WebAPI.Services
{
    public class EdtDateTimeProvider : IDateTimeProvider, IScoped
    {
        public DateTime Today => Now.Date;
        public DateTime Now => DateTime.UtcNow.AddHours(-4);
    }

    public class EstDateTimeProvider : IDateTimeProvider, IScoped
    {
        public DateTime Today => Now.Date;
        public DateTime Now => DateTime.UtcNow.AddHours(-5);
    }
}