using AutoMapper;
using Microsoft.Extensions.Configuration;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using PIOONEER_Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace PIOONEER_Service.Service
{
    public class ProductBUService : IProductBUService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Firebases _firebase;
        private readonly IMapper _mapper;

        public ProductBUService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper,Firebases firebase)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebase = firebase;
        }

        public IEnumerable<ProductBUResponseDTO> GetAllBUProduct()
        {
            IEnumerable<ProductByUser> ListProduct;
            ListProduct = _unitOfWork.ProductByUsers.Get().ToList();
            var ProductReposne = _mapper.Map<IEnumerable<ProductBUResponseDTO>>(ListProduct);
            return ProductReposne;
        }
        public async Task<ProductBUResponseDTO> GetProductBUByID(int id)
        {
            try
            {
                var product = _unitOfWork.ProductByUsers.Get(filter: c => c.Id == id).FirstOrDefault();

                if (product == null)
                {
                    throw new Exception("product not found");
                }

                var customerResponse = _mapper.Map<ProductBUResponseDTO>(product);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProductBUResponseDTO> CreateBUProduct(ProductBUDTO productAddDTO)
        {
        try
        {
            String imageUrl = await _firebase.UploadImage(productAddDTO.ProductUrlImg);
            var exiting = _unitOfWork.ProductByUsers.Get(C => C.ProductUrlImg == imageUrl).FirstOrDefault();

            if (exiting != null)
            {
                throw new Exception("Product with the same img already exists");
            }

            var product = _mapper.Map<ProductByUser>(productAddDTO);
                product.ProductUrlImg = imageUrl;


            _unitOfWork.ProductByUsers.Insert(product);
            await _unitOfWork.SaveChangesAsync();

            var ProductReposne = _mapper.Map<ProductBUResponseDTO>(product);

            return ProductReposne;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
        public async Task<ProductBUResponseDTO> UpdateBUProductBYID(int id, ProductBUDTO productUpdateDTO)
        {
            try
            {
                String imageUrl = await _firebase.UploadImage(productUpdateDTO.ProductUrlImg);
                var existingProduct = _unitOfWork.ProductByUsers.Get(C => C.Id == id).FirstOrDefault();
                if (existingProduct == null)
                {
                    throw new Exception("Product not found.");
                }
                var product = _mapper.Map(productUpdateDTO, existingProduct);
                product.ProductUrlImg = imageUrl;
                _unitOfWork.ProductByUsers.Update(existingProduct);
                await _unitOfWork.SaveChangesAsync();

                var productResponse =  _mapper.Map<ProductBUResponseDTO>(existingProduct);
                return productResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteBUProduct(int id)
        {
            try
            {
                var product = _unitOfWork.ProductByUsers.Get(filter: c => c.Id == id).FirstOrDefault();
                if (product == null)
                {
                    throw new Exception("User not found.");
                }

                _unitOfWork.ProductByUsers.Delete(product);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }






    }
}
