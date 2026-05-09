namespace ECommerce528.Models
{
    public class PromotionUserUsage
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductPromotionId { get; set; }
        public ProductPromotion ProductPromotion { get; set; }
        public string Code { get; set; }

        public DateTime UsedAt { get; set; }
    }
}
