using System;

namespace Todo.WebAPI.Domain
{
    public class DBRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Data { get; set; }
    }
}