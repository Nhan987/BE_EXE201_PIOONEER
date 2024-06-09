using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface IProductBUService
    {
        IEnumerable<ProductBUResponseDTO> GetAllBUProduct();
        Task<ProductBUResponseDTO> GetProductBUByID(int id);
        Task<ProductBUResponseDTO> CreateBUProduct(ProductBUDTO productAddDTO);
        Task<ProductBUResponseDTO> UpdateBUProductBYID(int id, ProductBUDTO productUpdateDTO);
        Task<bool> DeleteBUProduct(int id);
    }
}
