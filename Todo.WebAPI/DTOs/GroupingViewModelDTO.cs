using System.Collections.Generic;

namespace Todo.WebAPI.DTOs
{
    public class GroupingViewModelDTO
    {
        public int CompletedCount { get; set; }
        public IEnumerable<GroupingDTO> Groupings { get; set; }
    }
}