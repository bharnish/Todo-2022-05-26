using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Todo.Data
{
#if DEBUG
    [DynamoDBTable("todo-2022-05-26-dev")]
    public class NormalDB
    {
        [DynamoDBHashKey]
        public string DbKey { get; set; }

        public List<DBRecord> Records { get; set; }

        public DateTime Updated { get; set; } = DateTime.Now;
    }
#endif
}