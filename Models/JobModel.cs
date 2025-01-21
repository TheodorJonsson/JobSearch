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
        public bool Ongoing { get; set; }
        public ELevels? ELevel { get; set; }


        // Overrides the inherited equals function to only check for the ID for the item
        public override bool Equals(object? obj)
        {
            if(obj is JobModel b)
            {
                return this.Id == b.Id;
            }
            else
            {
                return false;
            }
        }
    }
}
