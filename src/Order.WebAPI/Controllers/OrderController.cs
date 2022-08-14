using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Model;
using Order.Service;
using Order.WebAPI.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.WebAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetOrdersByStatus/{statusId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdersByStatus(Guid statusId)
        {
            try
            {
                var order = await _orderService.GetOrdersByStatusIdAsync(statusId);
                if (order != null)
                    return Ok(order);

                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }


        }
        [HttpPut("UpdateOrderStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatus request)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(request.Id.Value, request.OrderStatusId.Value);
                if (result != null)
                    return Ok(result.Value);

                return NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPost("CreateOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] NewOrder request)
        {
            try
            {

                var result = await _orderService.CreateOrder(request);
                if (result != null)
                    return Ok(result.Value);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }


            return BadRequest();

        }
        [HttpGet("GetProfitByMonth/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfitByMonth(int month)
        {

            try
            {
                var result = await _orderService.GetProfitByMonth(month);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetProfitByYear/{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfitForAllMonthsofYear(int year)
        {
            try
            {
                var result = await _orderService.GetProfitForAllMonthsofYear(year);    //_orderService.GetProfitByYear(year);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


    }
}
