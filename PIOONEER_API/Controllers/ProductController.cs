using Humanizer;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        protected IUnitOfWork _unitOfWork;
        DateTime CreateDate = DateTime.Now;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var products = await _unitOfWork.Products.GetByIdAsync(id);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProducts(ProductAddDTO productDto)
        {
            if( productDto.CategoryId==null || productDto.DiscountId == null)
            {
                return BadRequest("Category and Discount must Enter");
            }

            var product = new Product
            {
                CategoryId = productDto.CategoryId,
                DiscountId = productDto.DiscountId,
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductPrice = productDto.ProductPrice,
                ProductQuantity = productDto.ProductQuantity,
                CreateDate = CreateDate,
                ModifiedDate = CreateDate,
                ProductUrlImg = productDto.ProductUrlImg,
                Status = true,
                ProductByUserId = productDto.ProductByUserId
            };

           await _unitOfWork.Products.AddAsync(product);
           await _unitOfWork.SaveChangesAsync();
           return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> updateProducts(int id, ProductUpdateDto productDto)
        {
            var existingProduct = await _unitOfWork.Products.FindAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var product = new Product
            {
                CategoryId = productDto.CategoryId,
                DiscountId = productDto.DiscountId,
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductPrice = productDto.ProductPrice,
                ProductQuantity = productDto.ProductQuantity,
                CreateDate = CreateDate,
                ModifiedDate = CreateDate,
                ProductUrlImg = productDto.ProductUrlImg,
                Status = true,
            };

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProducts(int id)
        {
            var products =await _unitOfWork.Products.GetByIdAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _unitOfWork.Products.Delete(products);
            await _unitOfWork.SaveChangesAsync();

            return Ok("the product was delete succesfully");
        }



    }
}
