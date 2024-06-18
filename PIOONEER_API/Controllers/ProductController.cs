using CoreApiResponse;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using PIOONEER_Service.Interface;
using PIOONEER_Service.Service;
using System.Net;
using Tools;
namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {

        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAllProduct([FromQuery] string searchQuery = null)
        {
            var product = _productService.GetAllProduct(searchQuery);
            return CustomResult("Data load successful", product);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByID(id);
                return CustomResult("product found", product);
            }
            catch (Exception ex)
            {
                return CustomResult("product not found", HttpStatusCode.NotFound);
            }
        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm]ProductAddDTO productAdd)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            var result = await _productService.CreateProduct(productAdd);
            if (result.Status != true)
            {
                return CustomResult("Create fail.", new { Productname = result.ProductName }, HttpStatusCode.Conflict);
            }
            return CustomResult("Create successful", result);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm]ProductUpdateDto productUpp)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _productService.UpdateProductBYID(id,productUpp);
                return CustomResult("Update successful", result);
            }
            catch (Exception ex)
            {
                return CustomResult("Update product fail", HttpStatusCode.BadRequest);
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
                if (result)
                {
                    return CustomResult("Delete successful.");
                }
                else
                {
                    return CustomResult("Product not found.", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return CustomResult("Delete fail.", HttpStatusCode.BadRequest);
            }
        }



    }
}
