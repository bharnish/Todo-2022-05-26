using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.WebAPI.DTOs
{
    public class GroupingDTO
    {
        public string Key { get; set; }
        public TodoDTO[] Data { get; set; }
    }
}
