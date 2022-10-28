using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Todo.Core;
using Todo.Data;
using Todo.Services;
using Todo.WebAPI.DTOs;
using Todo.WebAPI.Services;

namespace Todo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITodoParser _parser;
        private readonly ITodoMutator _mutator;
        private readonly ITodoFilter _todoFilter;
        private readonly IRepo _repo;

        private static string NoneKey = "~none~";

        public TasksController(IMapper mapper, ITodoParser parser, ITodoMutator mutator, ITodoFilter todoFilter, IRepo repo)
        {
            _mapper = mapper;
            _parser = parser;
            _mutator = mutator;
            _todoFilter = todoFilter;
            _repo = repo;
        }

        [FromHeader] public string DbKey { get; set; }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ViewModelDTO))]
        public async Task<IActionResult> Get([FromQuery] FilterOptionsDTO options)
        {
            var s = await _repo.Load(DbKey);

            var todos = s.Select(_parser.Parse).ToList();

            var completedCount = todos.Count(t => t.IsCompletedToday);

            var filtered = _todoFilter.Filter(todos, options);

            var rv = new ViewModelDTO
            {
                CompletedCount = completedCount,
                Todos = filtered.OrderBy(PrioritySort).Select(_mapper.Map<TodoDTO>)
            };

            return Ok(rv);
        }

        string PrioritySort(Data.Todo todo)
        {
            if (todo.Priority == null)
                return "zz";

            return todo.Priority;
        }

        [HttpGet("text")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetText()
        {
            var todos = await _repo.Load(DbKey);

            return Ok(string.Join("\n", todos.Select(x => x.Data)));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TodoDTO))]
        public async Task<IActionResult> Get(string id)
        {
            var records = await _repo.Load(DbKey);
            var record = records.FirstOrDefault(x => x.Id == id);
            if (record == null) return NotFound();

            var todo = _parser.Parse(record);

            return Ok(_mapper.Map<TodoDTO>(todo));

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StringDTO value, [FromQuery] bool replace = false)
        {
            var list = await _repo.Load(DbKey);

            var data = value.Value.Split('\n');

            var records =
                data
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(d => new DBRecord {Data = d});

            records = records.Select(_mutator.ReplaceRelativeDates);

            if (replace)
                list.Clear();

            list.AddRange(records);

            await _repo.Save(DbKey, list);

            return Ok();
        }

        [HttpPost("{id}/postpone")]
        public async Task<IActionResult> Postpone(string id, [FromQuery] int ndays)
        {
            var recs = await _repo.Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            _mutator.Postpone(rec, ndays);

            await _repo.Save(DbKey, recs);

            return Ok();
        }

        [HttpPost("{id}/postponeThreshold")]
        public async Task<IActionResult> PostponeThreshold(string id, [FromQuery] int ndays)
        {
            var recs = await _repo.Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            _mutator.PostponeThreshold(rec, ndays);

            await _repo.Save(DbKey, recs);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] StringDTO value)
        {
            var list = await _repo.Load(DbKey);
            var record = list.FirstOrDefault(x => x.Id == id);
            if (record == null) return NotFound();

            record.Data = value.Value;

            record = _mutator.ReplaceRelativeDates(record);

            await _repo.Save(DbKey, list);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var list = await _repo.Load(DbKey);

            list.RemoveAll(x => x.Id == id);

            await _repo.Save(DbKey, list);

            return Ok();
        }

        [HttpPut("{id}/completed")]
        public async Task<IActionResult> MarkCompleted(string id, [FromQuery] bool isCompleted)
        {
            var recs = await _repo.Load(DbKey);
            var rec = recs.FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();

            var newRecords = _mutator.MarkCompleted(rec, isCompleted);
            foreach (var dbRecord in newRecords)
                recs.Add(dbRecord);

            await _repo.Save(DbKey, recs);

            return Ok();
        }

        [HttpDelete("completed")]
        public async Task<IActionResult> DeleteCompleted()
        {
            var recs = await _repo.Load(DbKey);

            var todos = recs.Select(_parser.Parse).ToArray();

            foreach (var todo in todos.Where(x => x.IsCompleted))
            {
                recs.RemoveAll(x => x.Id == todo.Id);
            }

            await _repo.Save(DbKey, recs);

            return Ok();
        }

        [HttpGet("groupBy/{groupBy}")]
        [ProducesResponseType(200, Type = typeof(GroupingViewModelDTO))]
        public async Task<IActionResult> GroupBy([FromRoute] string groupBy, [FromQuery] FilterOptionsDTO options)
        {
            var recs = await _repo.Load(DbKey);

            var todos = recs.Select(_parser.Parse).ToList();

            var completedCount = todos.Count(t => t.IsCompletedToday);

            var filtered = _todoFilter.Filter(todos, options);

            var group = groupBy switch
            {
                "context" => Group(filtered, x => x.Contexts),
                "project" => Group(filtered, x => x.Projects),
                "dueDate" => Group(filtered,
                    x => new[] {x.DueDate == null ? "" : x.DueDate.Value.ToString(Patterns.DateFormat)}),
                "threshold" => Group(filtered,
                    x => new[] {x.ThresholdDate == null ? "" : x.ThresholdDate.Value.ToString(Patterns.DateFormat)}),
                "priority" => Group(filtered, x => new[] {x.Priority ?? ""}),
                _ => throw new Exception("Invalid GroupBy")
            };

            var rv =
                new GroupingViewModelDTO
                {
                    CompletedCount = completedCount,
                    Groupings = group.OrderBy(SortNoneLast)
                };

            return Ok(rv);

            string SortNoneLast(GroupingDTO g) => g.Key == NoneKey ? "zz" : g.Key;
        }

        private IEnumerable<GroupingDTO> Group(IEnumerable<Data.Todo> todos, Func<Data.Todo, IEnumerable<string>> func)
        {
            var d = new Dictionary<string, List<Data.Todo>>();

            foreach (var todo in todos.OrderBy(PrioritySort))
            {
                var proj = func(todo) ?? Enumerable.Empty<string>();

                var keys = proj.Select(x => x.ToLower()).ToArray();

                if (!keys.Any() || keys.All(string.IsNullOrEmpty))
                {
                    keys = new[] {NoneKey};
                }

                foreach (var key in keys)
                {
                    if (!d.TryGetValue(key, out var list))
                        d.Add(key, list = new List<Data.Todo>());
                    list.Add(todo);
                }
            }

            var gs =
                from kvp in d
                select new GroupingDTO
                {
                    Key = kvp.Key,
                    Data = kvp.Value.Select(_mapper.Map<TodoDTO>).ToArray(),
                    IsAllWaitingFor = kvp.Value.All(x => x.IsWaitingFor),
                };

            return gs;
        }

        [HttpGet("contexts")]
        public async Task<IEnumerable<string>> GetContexts()
        {
            var s = await _repo.Load(DbKey);
            var todos = s.Select(_parser.Parse).ToList();
            return todos.SelectMany(t => t.Contexts).Select(x => x.ToLower()).Distinct();
        }

        [HttpGet("projects")]
        public async Task<IEnumerable<string>> GetProjects()
        {
            var s = await _repo.Load(DbKey);
            var todos = s.Select(_parser.Parse).ToList();
            return todos.SelectMany(t => t.Projects).Select(x => x.ToLower()).Distinct();
        }
    }
}