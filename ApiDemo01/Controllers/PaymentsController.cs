using ApiDemo01.ResponseModule;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ApiDemo01.Controllers
{
    
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string WhSecret = "whsec_20147db827e202e8320311bb84da0fa02fa05445fbdeacd57d736d2077e8e499";

        public PaymentsController(IPaymentService paymentService,
                            ILogger<PaymentsController> logger) 
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [HttpPost("BasketId")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string baketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(baketId);
            if (basket is null)
                return BadRequest(new ApiResponce(400 , "Problem with your basket"));
            return Ok(basket);
        }
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"] , WhSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentPaymentFailed:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("payment Faild : ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFaild(intent.Id);
                    _logger.LogInformation("payment Faild : ", order.Id);

                    break;
                case Events.PaymentIntentSucceeded:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("payment Succeded : ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceded(intent.Id);
                    _logger.LogInformation("Order Updated to Payment Received : ", order.Id);
                    break;
            }
            return new EmptyResult();
        }
    }
}
