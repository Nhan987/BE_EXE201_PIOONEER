using Microsoft.AspNetCore.Http;
using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
     public interface IProductService
    {
        IEnumerable<ProductResponeDTO> GetAllProduct(string searchQuery = null);
        Task<ProductResponeDTO> GetProductByID(int id);
        Task<ProductResponeDTO> CreateProduct(ProductAddDTO productAddDTO);
        Task<ProductResponeDTO> UpdateProductBYID(int id,ProductUpdateDto productUpdateDTO);
        Task<bool> DeleteProduct(int id);
    }
}
