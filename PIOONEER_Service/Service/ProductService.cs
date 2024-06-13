using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Firebases _firebase;
        DateTime CreateDate = DateTime.Now;
        private readonly IMapper _mapper;

        public ProductService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, Firebases firebase)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebase = firebase;

        }
        public IEnumerable<ProductResponeDTO> GetAllProduct(string searchQuery = null)
        {
            IEnumerable<Product> ListProduct;
            if (string.IsNullOrEmpty(searchQuery))
            {
                ListProduct = _unitOfWork.Products.Get(filter: C=> C.Status == true).ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                ListProduct = _unitOfWork.Products.Get(filter: c => c.Status == true &&
                              (c.ProductName.ToLower().Contains(loweredSearchQuery) ||
                               c.ProductPrice.Equals(loweredSearchQuery)));
            }

            var ProductReposne = _mapper.Map<IEnumerable<ProductResponeDTO>>(ListProduct);
            return ProductReposne;
        }

        public async Task<ProductResponeDTO> GetProductByID(int id)
        {
            try
            {
                var product = _unitOfWork.Products.Get(filter: c => c.Id == id && c.Status == true).FirstOrDefault();

                if (product == null)
                {
                    throw new Exception("product not found");
                }

                var customerResponse = _mapper.Map<ProductResponeDTO>(product);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProductResponeDTO> CreateProduct(ProductAddDTO productAddDTO)
        {
            try
            {
                var exiting = _unitOfWork.Products.Get(C => C.ProductName == productAddDTO.ProductName).FirstOrDefault();
                String imageUrl = await _firebase.UploadImage(productAddDTO.ProductImg);
                if (exiting != null)
                {
                    throw new Exception("Product with the same name already exists");
                }

                var product = _mapper.Map<Product>(productAddDTO);
                product.Status = true;
                product.CreateDate = CreateDate;
                product.ModifiedDate = CreateDate;
                product.ProductUrlImg = imageUrl;

                _unitOfWork.Products.Insert(product);
                await _unitOfWork.SaveChangesAsync();

                var ProductReposne = _mapper.Map<ProductResponeDTO>(product);
                return ProductReposne;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ProductResponeDTO> UpdateProductBYID(int id, ProductUpdateDto productUpdateDTO)
        {
            try
            {
                var existingProduct = _unitOfWork.Products.Get(C => C.Id == id).FirstOrDefault();
                String imageUrl = await _firebase.UploadImage(productUpdateDTO.ProductImg);
                if (existingProduct == null)
                {
                    throw new Exception("Product not found.");
                }
                var product = _mapper.Map(productUpdateDTO, existingProduct);
                    product.ProductUrlImg =imageUrl;

                _unitOfWork.Products.Update(existingProduct);
                await _unitOfWork.SaveChangesAsync();

                var productResponse = _mapper.Map<ProductResponeDTO>(existingProduct);
                return productResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var product = _unitOfWork.Products.Get(filter: c => c.Id == id && c.Status == true).FirstOrDefault();
                if (product == null)
                {
                    throw new Exception("User not found.");
                }

                product.Status = false;
                _unitOfWork.Products.Update(product);
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
