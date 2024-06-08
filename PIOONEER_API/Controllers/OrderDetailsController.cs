using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using PIOONEER_Service.Interface;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : BaseController
    {
        private readonly IOrderDetailService _orderDetailService;
        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public  IActionResult GetAllOrder()
        {
            var orderDetail = _orderDetailService.GetAllOrderDetail();
            return CustomResult("Data load successful", orderDetail);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var orderDetail = await _orderDetailService.GetOrderDetailByID(id);
                return CustomResult("OrderDetail found", orderDetail);
            }
            catch (Exception ex)
            {
                return CustomResult("OrderDetail not found", HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] OrderDetailsAddDTO OrderDAdd)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            var result = await _orderDetailService.CreateOrderDetail(OrderDAdd);
            if (result == null)
            {
                return CustomResult("Create fail.", new { Ordercode = result }, HttpStatusCode.Conflict);
            }
            return CustomResult("Create successful", result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromForm] OrderDetailsUpDTO OrderDUp)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _orderDetailService.UpdateOrderDetailBYID(id, OrderDUp);
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
                var result = await _orderDetailService.DeleteOrderDetail(id);
                if (result)
                {
                    return CustomResult("Delete successful.");
                }
                else
                {
                    return CustomResult("OrderDetails not found.", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return CustomResult("Delete fail.", HttpStatusCode.BadRequest);
            }
        }

    }
}
