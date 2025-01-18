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

        public override bool Equals(object? obj)
        {
            if(obj is JobModel b)
            {
                return this.Id == b.Id && this.Position == b.Position && this.Company == b.Company && this.Location == b.Location;
            }
            else
            {
                return false;
            }
        }
    }
}
