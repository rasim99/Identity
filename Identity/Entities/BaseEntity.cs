namespace Identity.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CretaAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }
    }
}
