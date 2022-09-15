using System;
using Amazon.DynamoDBv2.DataModel;

namespace Todo.WebAPI.Domain
{
    [DynamoDBTable("todo-2022-05-26")]
    public class DB
    {
        [DynamoDBHashKey]
        public string DbKey { get; set; }

        public string Data { get; set; }
        public string Salt { get; set; }
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}