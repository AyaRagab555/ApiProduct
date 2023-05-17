using ApiDemo01.Dto;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo01.Controllers
{
    public class BasketController : BaseController
    {
        
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper) 
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        [HttpGet("GetBasket")]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }
        [HttpPost]   
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto custmerBasketDto)
        {
            var basket = _mapper.Map<CustomerBasket>(custmerBasketDto);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);
            return Ok(updatedBasket);
        }
        [HttpDelete]
        public async Task DeleteBasketById(string id)
            => await _basketRepository.DeleteBasketAsync(id);

    }
}
