using System;

namespace Todo.Data
{
    public class DBRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Data { get; set; }
    }
}