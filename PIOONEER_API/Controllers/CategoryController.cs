using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        protected IUnitOfWork _unitOfWork;
        private bool status = true;
        public CategoryController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategory()
        {
            var cate = _unitOfWork.Categories.GetAllAsync();
            return Ok(cate);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var cate = _unitOfWork.Categories.GetByIdAsync(id);
            return Ok(cate);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(CategoryAddDTO category)
        {
            if (category == null)
            {
                return BadRequest("You must enter all require fields");
            }
            var cate = new Category
            {
                CategoryName = category.CategoryName,
                Status = status
            };
            await _unitOfWork.Categories.AddAsync(cate);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new {id = cate.Id}, cate);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, CategoryUpdateDTO category)
        {
            var existCate = await _unitOfWork.Categories.FindAsync(x => x.Id == id);
            if (existCate == null)
            {
                return NotFound();
            }
            var cate = new Category
            {
                CategoryName = category.CategoryName,
                Status = category.Status
            };
            _unitOfWork.Categories.Update(cate);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = cate.Id }, cate);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var cate = await _unitOfWork.Categories.GetByIdAsync(id);
            if (cate == null)
            {
                return NotFound();
            }
            _unitOfWork.Categories.Delete(cate);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Delete Success!");
        }
    }
}
