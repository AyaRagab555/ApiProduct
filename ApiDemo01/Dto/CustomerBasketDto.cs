using Core.Entities;

namespace ApiDemo01.Dto
{
    public class CustomerBasketDto
    {
        public string Id { get; set; }
        public int? DeliveryMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public List<BasketItemDto> BasketItems { get; set; }
    }
}
