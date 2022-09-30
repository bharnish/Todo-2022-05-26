using System;
using System.Collections.Generic;
using System.Linq;

namespace Todo.Data
{
    public class Todo
    {
        public string Id { get; set; }
        public string Raw { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ThresholdDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool IsOverdue => DueDate < Today && !IsCompleted;
        public bool IsDueToday => DueDate == Today && !IsCompleted;
        public bool IsDueTomorrow => DueDate == Today.AddDays(1) && !IsCompleted;

        public bool IsThresholdPast => ThresholdDate < Today && !IsCompleted;
        public bool IsThresholdToday => ThresholdDate == Today && !IsCompleted;
        public bool IsFutureThreshold => ThresholdDate > Today && !IsCompleted;

        public bool IsCompleted => Raw.StartsWith("x ");
        public bool IsCompletedToday => CompletedDate == Today;

        public IEnumerable<string> Contexts { get; set; }
        public IEnumerable<string> Projects { get; set; }

        public string Priority { get; set; }
        public bool HasPriority => Priority != null && !IsCompleted;

        public bool IsWaitingFor => Contexts.Any(c => c.ToLower() == "@waitingfor");
        public bool IsProject => Contexts.Any(c => c.ToLower() == "@project");

        public DateTime Today { get; set; }
    }
}