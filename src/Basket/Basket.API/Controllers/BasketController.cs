using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketController> _logger;
        private readonly EventBusRabbitMQProducer _eventBus;

        public BasketController(IBasketRepository basketRepository, IMapper mapper, EventBusRabbitMQProducer eventBus, ILogger<BasketController> logger)
        {
            _eventBus = eventBus;
            _basketRepository = basketRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{username:minlength(1)}")]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBasket(string username)
        {
            try
            {
                var basket = await _basketRepository.GetBasket(username);

                if (basket != null)
                    return Ok(basket);

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateBasket([FromBody] BasketCart cart)
        {
            try
            {
                var basket = await _basketRepository.UpdateBasket(cart);

                if (basket != null)
                    return Ok(basket);

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{username:minlength(1)}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            try
            {
                var isDeleted = await _basketRepository.DeleteBasket(username);

                if (!isDeleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckoutBasket([FromBody] BasketCheckout basketCheckout)
        {
            try
            {
                var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
                if (basket == null)
                    return BadRequest();

                var isDeleted = await _basketRepository.DeleteBasket(basketCheckout.UserName);
                if (!isDeleted)
                    return BadRequest();

                var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
                eventMessage.RequestId = Guid.NewGuid();
                eventMessage.TotalPrice = basket.TotalPrice;

                _eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}