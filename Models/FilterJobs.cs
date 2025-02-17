namespace JobSearch.Models
{
    public class FilterJobs
    {
        public FilterJobs() { }
        public string? Position { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? SortBy { get; set; }
        public string? OrderBy { get; set; }
    }
}
