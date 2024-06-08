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
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        DateTime CreateDate = DateTime.Now;

        public OrderService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<OrderResponse> GetAllOrder(string searchQuery = null)
        {
            IEnumerable<Order> ListOrder;
            if (string.IsNullOrEmpty(searchQuery))
            {
                ListOrder = _unitOfWork.Orders.Get().ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                ListOrder = _unitOfWork.Orders.Get(filter: c => c.Status == "1" &&
                              (c.OrderCode.ToLower().Contains(loweredSearchQuery) ||
                               c.TotalPrice.Equals(loweredSearchQuery)));
            }

            var ProductReposne = _mapper.Map<IEnumerable<OrderResponse>>(ListOrder);
            return ProductReposne;
        }

        public async Task<OrderResponse> GetOrderByID(int id)
        {
            try
            {
                var order = _unitOfWork.Orders.Get(filter: c => c.Id == id && c.Status == "true").FirstOrDefault();

                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                var customerResponse = _mapper.Map<OrderResponse>(order);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrderResponse> CreateOrder(OrderAddDTO OrderAdd)
        {
            try
            {
                var exiting = _unitOfWork.Orders.Get(C => C.OrderCode == OrderAdd.OrderCode).FirstOrDefault();
                if (exiting != null)
                {
                    throw new Exception("Order with the same code already exists");
                }

                var order = _mapper.Map<Order>(OrderAdd);
                order.Status = "true";
                order.CreateDate = CreateDate;
                

                _unitOfWork.Orders.Insert(order);
                await _unitOfWork.SaveChangesAsync();

                var OrderReposne = _mapper.Map<OrderResponse>(order);
                return OrderReposne;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<OrderResponse> UpdateOrderBYID(int id,OrderUpDTO OrderUp)
        {
            try
            {
                var existingOrder = _unitOfWork.Orders.Get(C => C.Id == id).FirstOrDefault();

                if (existingOrder == null)
                {
                    throw new Exception("Order not found.");
                }
                _mapper.Map(OrderUp, existingOrder);
                _unitOfWork.Orders.Update(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                var orderResponse = _mapper.Map<OrderResponse>(existingOrder);
                return orderResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var order = _unitOfWork.Orders.Get(filter: c => c.Id == id && c.Status == "true").FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                order.Status = "false";
                _unitOfWork.Orders.Update(order);
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
