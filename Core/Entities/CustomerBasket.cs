
namespace Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket(string id) 
        {
            Id = id;
        }
        public string Id { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>(); 
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set;}
    }
}
