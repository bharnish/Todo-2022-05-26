using System.Collections.Generic;
using Todo.WebAPI.DTOs;

namespace Todo.WebAPI.Services
{
    public interface ITodoFilter
    {
        IEnumerable<Data.Todo> Filter(IEnumerable<Data.Todo> todos, FilterOptionsDTO options);
    }
}