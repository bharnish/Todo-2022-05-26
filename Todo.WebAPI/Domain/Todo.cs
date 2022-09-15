using System;
using System.Collections.Generic;

namespace Todo.WebAPI.Domain
{
    public class Todo
    {
        public string Id { get; set; }
        public string Raw { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ThresholdDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool IsOverdue
        {
            get
            {
                if (DueDate == null || IsCompleted) return false;
                return (DueDate.Value < Today);
            }
        }

        public bool IsDueToday
        {
            get
            {
                if (DueDate == null || IsCompleted) return false;
                return (DueDate.Value == Today);
            }
        }

        public bool IsDueTomorrow
        {
            get
            {
                if (DueDate == null || IsCompleted) return false;
                return (DueDate.Value == Today.AddDays(1));
            }
        }

        public bool IsThresholdToday
        {
            get
            {
                if (ThresholdDate == null || IsCompleted) return false;
                return (ThresholdDate.Value == Today);
            }
        }

        public bool IsThresholdPast
        {
            get
            {
                if (ThresholdDate == null || IsCompleted) return false;
                return (ThresholdDate.Value < Today);
            }
        }

        public bool IsCompleted => Raw.StartsWith("x ");
        public bool IsCompletedToday => CompletedDate != null && CompletedDate == Today;

        public bool IsFutureThreshold
        {
            get
            {
                if (ThresholdDate == null) return false;
                return ThresholdDate > Today;
            }
        }

        public IEnumerable<string> Contexts { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public string Priority { get; set; }
        public bool HasPriority => Priority != null && !IsCompleted;

        public DateTime Today { get; set; }
    }
}