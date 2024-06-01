using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIOONEER_API.Controllers
{
    public class DiscountController : Controller
    {
        protected IUnitOfWork _unitOfWork;
        private bool status = true;
        public DiscountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscount()
        {
            var disc = await _unitOfWork.Discounts.GetAllAsync();
            return Ok(disc);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscount(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var disc = _unitOfWork.Discounts.GetByIdAsync(id);
            return Ok(disc);
        }
        [HttpPost]
        public async Task<ActionResult<Discount>> AddDiscount(DiscountAddDTO discount)
        {
            if (discount == null)
            {
                return BadRequest("You must enter all require fields");
            }
            var disc = new Discount
            {
                ExpiredDay = discount.ExpiredDay,
                Percent = discount.Percent,
                Status = status
            };
            await _unitOfWork.Discounts.AddAsync(disc);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetDiscount", new { id = disc.Id }, disc);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Discount>> UpdateDiscount(DiscountUpdateDTO discount, int id)
        {
            var existDisc = await _unitOfWork.Discounts.FindAsync(x => x.Id == id);
            if (existDisc == null)
            {
                return NotFound();
            }
            var disc = new Discount
            {
                ExpiredDay = discount.ExpiredDay,
                Percent = discount.Percent,
                Status = discount.Status
            };
            _unitOfWork.Discounts.Update(disc);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetDiscount", new { id = disc.Id }, disc);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var disc = await _unitOfWork.Discounts.GetByIdAsync(id);
            if (disc == null)
            {
                return NotFound();
            }
            _unitOfWork.Discounts.Delete(disc);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Delete Success!");
        }
    }
}
