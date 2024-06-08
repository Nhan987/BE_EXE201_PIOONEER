using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using PIOONEER_Service.Interface;
using PIOONEER_Service.Service;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductByUserController : BaseController
    {
        private readonly IProductBUService _productBUService;

        public ProductByUserController(IProductBUService productBUService)
        {
            _productBUService = productBUService;
        }

        [HttpGet]
        public IActionResult GetAllBUProduct()
        {
            var product = _productBUService.GetAllBUProduct();
            return CustomResult("Data load successful", product);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productBUService.GetProductBUByID(id);
                return CustomResult("productBU found", product);
            }
            catch (Exception ex)
            {
                return CustomResult("productBU not found", HttpStatusCode.NotFound);
            }
        }
        /* thís code have a forgein key so it cancel
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductBUDTO productAdd)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            var result = await _productBUService.CreateBUProduct(productAdd);
            if (result.ProductUrlImg == null)
            {
                return CustomResult("Create fail.", new { ProductImg = result.ProductUrlImg}, HttpStatusCode.Conflict);
            }
            return CustomResult("Create successful", result);
        }
        */

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBUProduct(int id, [FromForm] ProductBUDTO productUpp)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _productBUService.UpdateBUProductBYID(id, productUpp);
                return CustomResult("Update successful", result);
            }
            catch (Exception ex)
            {
                return CustomResult("Update product fail", HttpStatusCode.BadRequest);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productBUService.DeleteBUProduct(id);
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
