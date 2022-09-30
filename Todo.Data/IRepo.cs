using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todo.Data
{
    public interface IRepo
    {
        Task<List<DBRecord>> Load(string dbkey);
        Task Save(string dbkey, IEnumerable<DBRecord> data);
    }
}