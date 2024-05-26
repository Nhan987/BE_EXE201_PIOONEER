using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : Controller
    {
        protected IUnitOfWork _unitOfWork;

        public OrderDetailsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderDetails()
        {
            var odersDetails = await _unitOfWork.OrderDetails.GetAllAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderDetails(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var orders = await _unitOfWork.OrderDetails.GetByIdAsync(id);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrders(OrderDetailsAddDTO OrderDetailsDto)
        {
            if (OrderDetailsDto == null)
            {
                return BadRequest("you must Enter All requiment");
            }

            var oderD = new OrderDetails
            {
                ProductId = OrderDetailsDto.ProductId,
                OrderId = OrderDetailsDto.OrderId,
                OrderQuantity = OrderDetailsDto.OrderQuantity,
                OrderPrice = OrderDetailsDto.OrderPrice
            };

            await _unitOfWork.OrderDetails.AddAsync(oderD);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetOrderDetails", new { id = oderD.Id }, oderD);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDetails>> updateOrders(int id, OrderDetailsUpDTO OrderDTO)
        {
            var existingProduct = await _unitOfWork.OrderDetails.FindAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var orderD = new OrderDetails
            {
                OrderQuantity= OrderDTO.OrderQuantity,
                OrderPrice= OrderDTO.OrderPrice
            };

            _unitOfWork.OrderDetails.Update(orderD);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = orderD.Id }, orderD);
        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteOrderDetails(int id)
        {
            var orderD = await _unitOfWork.OrderDetails.GetByIdAsync(id);
            if (orderD == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderDetails.Delete(orderD);
            await _unitOfWork.SaveChangesAsync();

            return Ok("the Order was delete succesfully");
        }
    }
}
