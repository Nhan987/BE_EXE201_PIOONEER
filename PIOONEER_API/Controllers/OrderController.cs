using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        protected IUnitOfWork _unitOfWork;
        DateTime CreateDate = DateTime.Now;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetOrder()
        {
            var oders = await _unitOfWork.Orders.GetAllAsync();
            return Ok(oders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var orders = await _unitOfWork.Orders.GetByIdAsync(id);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrders(OrderAddDTO OrderDto)
        {
            if (OrderDto == null)
            {
                return BadRequest("you must Enter All requiment");
            }

            var oder = new Order
            {
                UserId = OrderDto.UserId,
                OrderRequirement = OrderDto.OrderRequirement,
                OrderCode = OrderDto.OrderCode,
                PaymentMethod = OrderDto.PaymentMethod,
                CreateDate = CreateDate,
                TotalPrice = OrderDto.TotalPrice,
                Status = "true"
            };

            await _unitOfWork.Orders.AddAsync(oder);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = oder.Id }, oder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> updateOrders(int id, OrderUpDTO OrderUPDTO)
        {
            var existingProduct = await _unitOfWork.Orders.FindAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                OrderRequirement = OrderUPDTO.OrderRequirement,
                OrderCode = OrderUPDTO.OrderCode,
                PaymentMethod = OrderUPDTO.PaymentMethod,
                TotalPrice= OrderUPDTO.TotalPrice,
                Status = OrderUPDTO.Status
            };

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteOrder(int id)
        {
            var orders = await _unitOfWork.Orders.GetByIdAsync(id);
            if (orders == null)
            {
                return NotFound();
            }

            _unitOfWork.Orders.Delete(orders);
            await _unitOfWork.SaveChangesAsync();

            return Ok("the Order was delete succesfully");
        }
    }
}
