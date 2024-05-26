using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductByUserController : Controller
    {
        protected IUnitOfWork _unitOfWork;

        public ProductByUserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductByUser>>> GetProductBU()
        {
            var productBU = await _unitOfWork.ProductByUsers.GetAllAsync();
            return Ok(productBU);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductByUser>>> GetProductBU(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var productBU = await _unitOfWork.ProductByUsers.GetByIdAsync(id);
            return Ok(productBU);
        }

        [HttpPost]
        public async Task<ActionResult<ProductByUser>> PostProductBU(ProductBUDTO productBUDto)
        {
            if (productBUDto != null)
            {
                return BadRequest("URL IMG can not be null");
            }

            var productBU = new ProductByUser
            {
                ProductUrlImg =  productBUDto.ProductUrlImg
            };

            await _unitOfWork.ProductByUsers.AddAsync(productBU);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetProductBU", new { id = productBU.Id }, productBU);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductByUser>> updateProductBU(int id, ProductBUDTO productBUDto)
        {
            var existingProduct = await _unitOfWork.ProductByUsers.FindAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            
            var productBU = new ProductByUser
            {
                ProductUrlImg = productBUDto.ProductUrlImg
            };

            _unitOfWork.ProductByUsers.Update(productBU);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetProductBU", new { id = productBU.Id }, productBU);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductBU(int id)
        {
            var productBU = await _unitOfWork.ProductByUsers.GetByIdAsync(id);
            if (productBU == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductByUsers.Delete(productBU);
            await _unitOfWork.SaveChangesAsync();

            return Ok("the productBU was delete succesfully");
        }
    }
}
