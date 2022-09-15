namespace Todo.WebAPI.DTOs
{
    public class FilterOptionsDTO
    {
        public bool Completed { get; set; }
        public bool Future { get; set; }
        public string Filters { get; set; }
    }
}