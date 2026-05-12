namespace ECommerce528.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public string Img { get; set; }
    }
}
