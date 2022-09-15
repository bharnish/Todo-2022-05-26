using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Todo.WebAPI.Domain;
using Todo.WebAPI.DTOs;
using Todo.WebAPI.Services;

namespace Todo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;
        private readonly TodoParser _parser;
        private readonly TodoMutator _mutator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TasksController(IDynamoDBContext context, IMapper mapper, TodoParser parser, TodoMutator mutator,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _mapper = mapper;
            _parser = parser;
            _mutator = mutator;
            _dateTimeProvider = dateTimeProvider;
        }

        [FromHeader] public string DbKey { get; set; }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ViewModelDTO))]
        public async Task<IActionResult> Get([FromQuery] FilterOptionsDTO options)
        {
            var s = await Load(DbKey);

            var todos = s.Select(_parser.Parse);

            var completedCount = todos.Count(t => t.IsCompletedToday);

            todos = Filter(todos, options);

            var rv = new ViewModelDTO
            {
                CompletedCount = completedCount,
                Todos = todos.Select(_mapper.Map<TodoDTO>)
            };

            return Ok(rv);
        }

        string PrioritySort(Domain.Todo todo)
        {
            if (todo.Priority == null)
                return "z";

            return todo.Priority;
        }

        [HttpGet("text")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetText([FromQuery] FilterOptionsDTO options)
        {
            var s = await Load(DbKey);

            var todos = s.Select(_parser.Parse);

            todos = Filter(todos, options);

            return Ok(string.Join("\n", todos.Select(x => x.Raw)));
        }

        IEnumerable<Domain.Todo> Filter(IEnumerable<Domain.Todo> todos, FilterOptionsDTO options)
        {
            if (!string.IsNullOrEmpty(options.Filters))
            {
                var filters = options.Filters.Split('\n').Select(x => x.ToLower());

                foreach (var filter in filters)
                {
                    if (filter.StartsWith("-"))
                        todos = todos.Where(x => !x.Raw.ToLower().Contains(filter.Substring(1)));
                    else
                        todos = todos.Where(x => x.Raw.ToLower().Contains(filter));
                }
            }

            if (!options.Completed)
                todos = todos.Where(x => !x.IsCompleted);

            if (!options.Future)
                todos = todos.Where(x => !x.IsFutureThreshold);

            todos = todos.OrderBy(x => x.IsCompleted);

            return todos;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TodoDTO))]
        public async Task<IActionResult> Get(string id)
        {
            var records = await Load(DbKey);
            var record = records.FirstOrDefault(x => x.Id == id);
            if (record == null) return NotFound();

            var todo = _parser.Parse(record);

            return Ok(_mapper.Map<TodoDTO>(todo));

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StringDTO value, [FromQuery] bool replace = false)
        {
            var list = await Load(DbKey);

            var data = value.Value.Split('\n');

            var records =
                data
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(d => new DBRecord {Data = d});

            records = records.Select(_mutator.ReplaceRelativeDates);

            if (replace)
                list.Clear();

            list.AddRange(records);

            await Save(DbKey, list);

            return Ok();
        }

        [HttpPost("{id}/postpone")]
        public async Task<IActionResult> Postpone(string id, [FromQuery] int ndays)
        {
            var recs = await Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            _mutator.Postpone(rec, ndays);

            await Save(DbKey, recs);

            return Ok();
        }

        [HttpPost("{id}/postponeThreshold")]
        public async Task<IActionResult> PostponeThreshold(string id, [FromQuery] int ndays)
        {
            var recs = await Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            _mutator.PostponeThreshold(rec, ndays);

            await Save(DbKey, recs);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] StringDTO value)
        {
            var list = await Load(DbKey);
            var record = list.FirstOrDefault(x => x.Id == id);
            if (record == null) return NotFound();

            record.Data = value.Value;

            record = _mutator.ReplaceRelativeDates(record);

            await Save(DbKey, list);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var list = await Load(DbKey);

            list.RemoveAll(x => x.Id == id);

            await Save(DbKey, list);

            return Ok();
        }

        [HttpPut("{id}/completed")]
        public async Task<IActionResult> MarkCompleted(string id, [FromQuery] bool isCompleted)
        {
            var recs = await Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            var newRecords = _mutator.MarkCompleted(rec, isCompleted);
            foreach (var dbRecord in newRecords)
                recs.Add(dbRecord);

            await Save(DbKey, recs);

            return Ok();
        }

        [HttpDelete("completed")]
        public async Task<IActionResult> DeleteCompleted()
        {
            var recs = await Load(DbKey);

            var todos = recs.Select(_parser.Parse).ToArray();

            foreach (var todo in todos.Where(x => x.IsCompleted))
            {
                recs.RemoveAll(x => x.Id == todo.Id);
            }

            await Save(DbKey, recs);

            return Ok();
        }

        [HttpGet("groupBy/{groupBy}")]
        [ProducesResponseType(200, Type = typeof(GroupingViewModelDTO))]
        public async Task<IActionResult> GroupBy([FromRoute] string groupBy, [FromQuery] FilterOptionsDTO options)
        {
            var recs = await Load(DbKey);

            var todos = recs.Select(_parser.Parse);

            var completedCount = todos.Count(t => t.IsCompletedToday);

            todos = Filter(todos, options);

            IEnumerable<GroupingDTO> group;

            if (groupBy == "context")
                group = Group(todos, x => x.Contexts);
            else if (groupBy == "project")
                group = Group(todos, x => x.Projects);
            else if (groupBy == "dueDate")
                group = Group(todos,
                    x => new[] {x.DueDate == null ? "" : x.DueDate.Value.ToString(Patterns.DateFormat)});
            else if (groupBy == "threshold")
                group = Group(todos,
                    x => new[] {x.ThresholdDate == null ? "" : x.ThresholdDate.Value.ToString(Patterns.DateFormat)});
            else if (groupBy == "priority")
                group = Group(todos, x => new[] {x.Priority ?? ""});
            else
                return BadRequest();

            var rv =
                new GroupingViewModelDTO
                {
                    CompletedCount = completedCount,
                    Groupings = group.OrderBy(g => g.Key)
                };

            return Ok(rv);
        }

        private IEnumerable<GroupingDTO> Group(IEnumerable<Domain.Todo> todos, Func<Domain.Todo, IEnumerable<string>> func)
        {
            var d = new Dictionary<string, List<Domain.Todo>>();

            foreach (var todo in todos.OrderBy(PrioritySort))
            {
                var proj = func(todo) ?? Enumerable.Empty<string>();

                var keys = proj.Select(x => x.ToLower()).ToArray();

                if (!keys.Any() || keys.All(string.IsNullOrEmpty))
                {
                    keys = new[] {"-none-"};
                }

                foreach (var key in keys)
                {
                    if (!d.TryGetValue(key, out var list))
                        d.Add(key, list = new List<Domain.Todo>());
                    list.Add(todo);
                }
            }

            var gs =
                from kvp in d
                select new GroupingDTO
                {
                    Key = kvp.Key,
                    Data = kvp.Value.Select(_mapper.Map<TodoDTO>).ToArray()
                };

            return gs;
        }

        private async Task<List<DBRecord>> Load(string dbkey)
        {
            var id = BuildId(dbkey);
            var db = await _context.LoadAsync<DB>(id);
            if (db == null) return new List<DBRecord>();

            var (k, iv) = GetKeyAndIV(dbkey, db.Salt);

            return await Decrypt(db.Data, k, iv);
        }

        private async Task Save(string dbkey, IEnumerable<DBRecord> data)
        {
            var id = BuildId(DbKey);

            if (!data.Any())
            {
                await RemoveRecord();
                return;
            }

            var salt = GenerateSalt();
            var (k, iv) = GetKeyAndIV(DbKey, salt);

            var s = await Encrypt(data, k, iv);

            var db = new DB
            {
                DbKey = id,
                Data = s,
                Salt = salt,
                Updated = DateTime.Now,
            };

            await _context.SaveAsync(db);

            async Task RemoveRecord()
            {
                var tmp = new DB
                {
                    DbKey = id,
                };
                await _context.DeleteAsync(tmp);
            }
        }

        static async Task<List<DBRecord>> Decrypt(string input, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;


            await using var ms = new MemoryStream(Decode(input));
            await using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var lines = new List<DBRecord>();
            while (true)
            {
                var id = await sr.ReadLineAsync();
                if (id == null) break;

                var data = await sr.ReadLineAsync();
                lines.Add(new DBRecord {Id = id, Data = data});
            }

            return lines;
        }

        static async Task<string> Encrypt(IEnumerable<DBRecord> input, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            await using var ms = new MemoryStream();
            await using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            await using (var sw = new StreamWriter(cs))
            {
                foreach (var line in input)
                {
                    await sw.WriteLineAsync(line.Id);
                    await sw.WriteLineAsync(line.Data);
                }
            }

            return Encode(ms.ToArray());
        }

        static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();

            var buffer = new byte[SALT_LEN_BYTES];

            rng.GetBytes(buffer);

            return Encode(buffer);
        }

        const int SALT_LEN_BYTES = 16;
        const int IV_LEN_BYTES = 16;
        const int KEY_LEN_BYTES = 32;

        static (byte[], byte[]) GetKeyAndIV(string password, string salt)
        {
            var saltBytes = Decode(salt);
            using var pbkdf = new Rfc2898DeriveBytes(password, saltBytes, 10000);

            var key = pbkdf.GetBytes(KEY_LEN_BYTES);
            var iv = pbkdf.GetBytes(IV_LEN_BYTES);

            return (key, iv);
        }

        static string Encode(byte[] input) => Convert.ToBase64String(input);
        static byte[] Decode(string input) => Convert.FromBase64String(input);

        static string BuildId(string key)
        {
            var buffer = Encoding.UTF8.GetBytes(key);

            using var sha1 = SHA1.Create();
            var hashed = sha1.ComputeHash(buffer);

            return Encode(hashed);
        }
    }
}