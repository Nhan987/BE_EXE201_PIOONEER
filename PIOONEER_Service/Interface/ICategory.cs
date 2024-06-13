using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface ICategory
    {
        IEnumerable<CategoryResponse> GetAll(string searchQuery = null);
        Task<CategoryResponse> GetById(int id);
        Task<CategoryResponse> Create(CategoryRequest cateRequest);
        Task<CategoryResponse> Update(int id, CategoryRequest cateRequest);
        Task<bool> Delete(int id);
    }
}
