namespace Todo.WebAPI.Domain
{
    public static class Patterns
    {
        public const string PriorityPattern = @"^(?<priority>\([A-Z]\)\s)";
        public const string ProjectPattern = @"(?<proj>(?<=^|\s)\+[^\s]+)";
        public const string ContextPattern = @"(^|\s)(?<context>\@[^\s]+)";
        public const string CompletedPattern = @"^X\s((\d{4})-(\d{2})-(\d{2}))?";
        public const string DueDatePattern = @"due:(?<date>(\d{4})-(\d{2})-(\d{2}))";
        public const string ThresholdDatePattern = @"t:(?<date>(\d{4})-(\d{2})-(\d{2}))";
        public const string RecurPattern = @"rec:(?<strict>\+?)(?<quantity>\d+)(?<period>[dwmy])";
        public const string RelativeDatePattern = @"(?<quantity>\d+)(?<period>[dwmy])";

        public const string TodoNextPattern = @"to:""(?<item>[^""]+)""";

        public const string DateFormat = @"yyyy-MM-dd";
    }
}