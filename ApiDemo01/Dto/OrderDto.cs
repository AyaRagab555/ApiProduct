using Core.Entities.OrderAggregate;

namespace ApiDemo01.Dto
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public ShippingAddressDto Address { get; set; }
    }
}
