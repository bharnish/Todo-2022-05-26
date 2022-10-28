using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Todo.Core;

namespace Todo.Data
{
#if DEBUG
    public class NormalRepo : IScoped, IRepo
    {
        private readonly IDynamoDBContext _context;

        public NormalRepo(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<List<DBRecord>> Load(string dbkey)
        {
            var db = await _context.LoadAsync<NormalDB>(dbkey);
            if (db == null) return new List<DBRecord>();

            return db.Records;
        }

        public async Task Save(string dbkey, IEnumerable<DBRecord> data)
        {
            var db = new NormalDB
            {
                DbKey = dbkey,
                Records = data.ToList(),
            };

            await _context.SaveAsync(db);
        }
    }
#endif
}