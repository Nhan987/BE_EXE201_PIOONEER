using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Service.Interface;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAllOrder([FromQuery] string searchQuery = null)
        {
            var order = _orderService.GetAllOrder(searchQuery);
            return CustomResult("Data load successful", order);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByID(id);
                return CustomResult("Order found", order);
            }
            catch (Exception ex)
            {
                return CustomResult("Order not found", HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] OrderAddDTO OrderAdd)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            var result = await _orderService.CreateOrder(OrderAdd);
            if (result.Status != "true")
            {
                return CustomResult("Create fail.", new { Ordercode = result.OrderCode }, HttpStatusCode.Conflict);
            }
            return CustomResult("Create successful", result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromForm] OrderUpDTO OrderUp)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _orderService.UpdateOrderBYID(id,OrderUp);
                return CustomResult("Update successful", result);
            }
            catch (Exception ex)
            {
                return CustomResult("Update Order fail", HttpStatusCode.BadRequest);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrder(id);
                if (result)
                {
                    return CustomResult("Delete successful.");
                }
                else
                {
                    return CustomResult("Order not found.", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return CustomResult("Delete fail.", HttpStatusCode.BadRequest);
            }
        }
    }
}
