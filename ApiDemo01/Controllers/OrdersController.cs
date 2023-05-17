using ApiDemo01.Dto;
using ApiDemo01.Extensions;
using ApiDemo01.ResponseModule;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo01.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService
            , IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var address = _mapper.Map<ShippingAddress>(orderDto.Address);
            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);
            if (order is null)
                return BadRequest(new ApiResponce(400, "Problem When Creating Order!!"));

            return Ok(order);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var order = await _orderService.GetOrderByIdAsync(id, email);
            if (order == null)
                return NotFound(new ApiResponce(404, "order does not Exist"));
            return Ok(_mapper.Map<OrderDetailsDto>(order));
        }

        [HttpGet("GetAllOrdersForUser")]
        public async Task<ActionResult<IReadOnlyList<OrderDetailsDto>>> GetOrdersForUser()
        {
             var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var order = await _orderService.GetOrderForUserAsync(email);
            return Ok(_mapper.Map<IReadOnlyList<OrderDetailsDto>>(order));
        }

        [HttpGet("DeliveryMethod")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
            => Ok(await _orderService.GetDeliveryMethodsAsync());
    }
}
