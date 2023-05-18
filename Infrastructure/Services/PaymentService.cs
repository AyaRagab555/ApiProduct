using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork,
                IBasketRepository basketRepository,
                IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _config = config;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
                return null;
            var shippingPrice = 0m;
            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Price;
            }
            foreach(var item in basket.BasketItems)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                if(item.Price != productItem.Price)
                    item.Price = productItem.Price;
            }
            var services = new PaymentIntentService();
            PaymentIntent intent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await services.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                };
                await services.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFaild(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);
            if (order == null)
                return null;

            order.OrderStatus = OrderStatus.PaymentFaild;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);
            if (order == null)
                return null;

            order.OrderStatus = OrderStatus.PaymentRecieved;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }
    }
}
