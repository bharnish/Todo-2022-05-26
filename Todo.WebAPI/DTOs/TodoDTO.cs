using System.Collections.Generic;

namespace Todo.WebAPI.DTOs
{
    public class TodoDTO
    {
        public string Id { get; set; }
        public string Raw { get; set; }

        public bool IsOverdue { get; set; }
        public bool IsDueToday { get; set; }
        public bool IsDueTomorrow { get; set; }

        public bool IsCompleted { get; set; }

        public string Priority { get; set; }
        public bool HasPriority { get; set; }

        public bool IsWaitingFor { get; set; }
        public bool IsProject { get; set; }

        public bool IsThresholdToday { get; set; }
        public bool IsThresholdPast { get; set; }

        public List<string> Contexts { get; set; }
        public List<string> Projects { get; set; }
    }
}