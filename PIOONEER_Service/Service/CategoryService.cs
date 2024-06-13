using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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

namespace PIOONEER_Service.Service
{
    public class CategoryService : ICategory
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> Create(CategoryRequest cateRequest)
        {
            try
            {
                var cate = _mapper.Map<Category>(cateRequest);
                cate.CategoryName = cateRequest.CategoryName;
                cate.Status = true;

                _unitOfWork.Categories.Insert(cate);
                _unitOfWork.Save();

                var cateResponse = _mapper.Map<CategoryResponse>(cate);
                return cateResponse;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var cate = _unitOfWork.Categories.GetByID(id);
                if (cate == null)
                {
                    throw new Exception("Category not found.");
                }
                cate.Status = false;
                _unitOfWork.Categories.Update(cate);
                _unitOfWork.Save();

                return true;
            } catch(Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<CategoryResponse> GetAll(string searchQuery = null)
        {
            IEnumerable<Category> listCate;
            if (string.IsNullOrEmpty(searchQuery))
            {
                listCate = _unitOfWork.Categories.Get(filter: c => c.Status.Equals(true)).ToList();
            }
            else
            {
                var searchQuerry2 = searchQuery.ToLower();
                listCate = _unitOfWork.Categories.Get(filter: c => c.Status.Equals(true) &&
                                                        c.CategoryName.ToLower().Contains(searchQuerry2)).ToList();
            }
            var cateResponse = _mapper.Map<IEnumerable<CategoryResponse>>(listCate);
            return cateResponse;
        }

        public async Task<CategoryResponse> GetById(int id)
        {
            try
            {
                var cate = _unitOfWork.Categories.Get(filter: c => c.Id == id && c.Status.Equals(true)).FirstOrDefault();
                if (cate == null)
                {
                    throw new Exception("No category found!!");
                }
                var cateResponse = _mapper.Map<CategoryResponse>(cate);
                return cateResponse;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CategoryResponse> Update(int id, CategoryRequest cateRequest)
        {
            try
            {
                var existCate = _unitOfWork.Categories.GetByID(id);
                if (existCate == null)
                {
                    throw new Exception("Category not found.");
                }

                _mapper.Map(cateRequest, existCate);
                _unitOfWork.Categories.Update(existCate);
                _unitOfWork.Save();

                var cateResponse = _mapper.Map<CategoryResponse>(existCate);
                return cateResponse;
            } catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
