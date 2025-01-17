namespace JobSearch.Models
{
    public class JobModel
    {
        public int Id { get; set; }
        public string? Position { get; set; }
        public string? Company {  get; set; }
        public string? Location {  get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
    }
}
