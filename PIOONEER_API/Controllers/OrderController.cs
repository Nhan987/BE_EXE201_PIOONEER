using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PIOONEER_Model.DTO;
using PIOONEER_Service.Interface;
using PIOONEER_Service.Service;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
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

        [HttpGet("{id:int}")]
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

        [HttpGet("orders/{mail}")]
        public IActionResult GetOrderByMail(string mail)
        {
            var order = _orderService.GetAllOrderByEmail(mail);
            return CustomResult("Order found", order);
        }

        [HttpGet("orders/send/{mail}")]
        public IActionResult GetOrderByMailAndSend(string mail)
        {
            var order = _orderService.GetAllOrderByEmailAndSendEmailAsync(mail);
            return CustomResult("Order found", order);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] userAndOrderDTO uo)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }

            var result = await _orderService.CreateUserOrder(uo);
            
            if (result == null)
            {
                return CustomResult("Create fail.", new { OrderBuser = result }, HttpStatusCode.Conflict);
            }

            return CustomResult("Create successful", result);
        }

        [HttpPost("create-order-details")]
        public async Task<IActionResult> CreateOrderOrdetails([FromBody] userAndOrderAndOrderdetailsDTO uo)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }

            var result = await _orderService.AssignOrderdetails(uo);

            if (result != null)
            {
                return CustomResult("Create fail.", new { OrderBuser = result }, HttpStatusCode.Conflict);
            }

            return CustomResult("Create successful", result);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateOrder(string code, [FromBody] OrderUpDTO OrderUp)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _orderService.UpdateOrderByCode(code,OrderUp);
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
