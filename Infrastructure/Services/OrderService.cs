using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
  

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, ShippingAddress address)
        {
            //Get Basket
            var basket = await _basketRepository.GetBasketAsync(basketId);
            //Get BasketItems from Product Repo

            var items = new List<OrderItem>();
            foreach (var item in basket.BasketItems)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrderd = new ProductItemOrderd(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrderd, productItem.Price, item.Quantity);
                items.Add(orderItem);

            }
            //Get DeliveryMethod
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            //calculate SubTotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);
            //Todo => payment
            //check to see if order exists
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);
            
            if(existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            }

            //create Order
            var order = new Order(buyerEmail , address , deliveryMethod, items , subTotal ,basket.PaymentIntentId);
            _unitOfWork.Repository<Order>().Add(order);
            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return null;

            //Delete Basket
            //await _basketRepository.DeleteBasketAsync(basketId);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync(); 

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var orderSpec = new OrderWithItemsSpecifications(id , buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(orderSpec);

        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            var orderSpec = new OrderWithItemsSpecifications( buyerEmail);
            return await _unitOfWork.Repository<Order>().GetListAsync(orderSpec);

        }
    }
}
