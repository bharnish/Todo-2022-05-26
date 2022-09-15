using System.Collections.Generic;

namespace Todo.WebAPI.DTOs
{
    public class ViewModelDTO
    {
        public int CompletedCount { get; set; }
        public IEnumerable<TodoDTO> Todos { get; set; }
    }
}