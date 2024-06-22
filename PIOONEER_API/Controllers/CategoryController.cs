using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Service.Interface;
using PIOONEER_Service.Service;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategory _service;

        public CategoryController(ICategory service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllCategory([FromQuery] string searchQuery = null)
        {
            var cate = _service.GetAll(searchQuery);
            return Ok(cate);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var cate = _service.GetById(id);
                return CustomResult("Data load successful", cate);
            }
            catch (Exception ex)
            {
                return CustomResult("Category not found", HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest cateRequest)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            var result = await _service.Create(cateRequest);
            if (!result.Status.Equals(true)) {
                return CustomResult("Create fail.", new { CategoryName = result.CategoryName }, HttpStatusCode.Conflict);
            }
            return CustomResult("Create successful", result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest cateRequest)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _service.Update(id, cateRequest);
                return CustomResult("Update successful", result);
            } catch (Exception ex)
            {
                return CustomResult("Update fail", HttpStatusCode.BadRequest);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _service.Delete(id);
                if (result)
                {
                    return CustomResult("Delete successful.");
                }
                else
                {
                    return CustomResult("Category not found.", HttpStatusCode.NotFound);
                }
            } catch (Exception ex)
            {
                return CustomResult("Delete fail.", HttpStatusCode.BadRequest);
            }
        }
    }
}
