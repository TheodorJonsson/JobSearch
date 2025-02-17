namespace JobSearch.Models
{
    public class JobModel
    {
        public JobModel() { }
        public int JobId { get; set; }
        public string? Position { get; set; }
        public string? Company {  get; set; }
        public string? Location {  get; set; }
        public string? Description { get; set; }
        public DateOnly? Date { get; set; }
        public bool Ongoing { get; set; }
        public string? ELevel { get; set; }
        // Overrides the inherited equals function to only check for the ID for the item
        public override bool Equals(object? obj)
        {
            if(obj is JobModel b)
            {
                if (!b.Position.Equals(this.Position) || !b.Company.Equals(this.Company) || !b.Location.Equals(this.Location))
                {
                    return false; 
                } 
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
