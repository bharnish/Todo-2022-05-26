using System.Collections.Generic;
using System.Linq;
using Todo.WebAPI.DTOs;

namespace Todo.WebAPI.Services
{
    public class TodoFilter : ITodoFilter
    {
        public IEnumerable<Data.Todo> Filter(IEnumerable<Data.Todo> todos, FilterOptionsDTO options)
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
    }
}
