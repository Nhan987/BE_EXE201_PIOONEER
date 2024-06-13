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

namespace PIOONEER_Service.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        DateTime CreateDate = DateTime.Now;
        public OrderDetailService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<OrderDetailsResponse> GetAllOrderDetail()
        {
            IEnumerable<OrderDetails> ListOrderDetails;

            ListOrderDetails = _unitOfWork.OrderDetails.Get().ToList();
           
            var ProductReposne = _mapper.Map<IEnumerable<OrderDetailsResponse>>(ListOrderDetails);
            return ProductReposne;
        }
        public async Task<OrderDetailsResponse> GetOrderDetailByID(int id)
        {
            try
            {
                var orderDetail = _unitOfWork.OrderDetails.Get(filter: c => c.Id == id).FirstOrDefault();

                if (orderDetail == null)
                {
                    throw new Exception("OrderDetails not found");
                }

                var customerResponse = _mapper.Map<OrderDetailsResponse>(orderDetail);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrderDetailsResponse> CreateOrderDetail(OrderDetailsAddDTO orderDetailsAdd)
        {
            try
            {

                var order = _mapper.Map<OrderDetails>(orderDetailsAdd);
                _unitOfWork.OrderDetails.Insert(order);
                await _unitOfWork.SaveChangesAsync();

                var OrderReposne = _mapper.Map<OrderDetailsResponse>(order);
                return OrderReposne;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrderDetailsResponse> UpdateOrderDetailBYID(int id, OrderDetailsUpDTO OrderDUp)
        {
            try
            {
                var existingOrder = _unitOfWork.OrderDetails.Get(C => C.Id == id).FirstOrDefault();

                if (existingOrder == null)
                {
                    throw new Exception("Order not found.");
                }
                _mapper.Map(OrderDUp, existingOrder);
                _unitOfWork.OrderDetails.Update(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                var orderResponse = _mapper.Map<OrderDetailsResponse>(existingOrder);
                return orderResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteOrderDetail(int id)
        {
            try
            {
                var orderD = _unitOfWork.OrderDetails.Get(filter: c => c.Id == id).FirstOrDefault();
                if (orderD == null)
                {
                    throw new Exception("OrderDetails not found.");
                }

                ;
                _unitOfWork.OrderDetails.Delete(orderD);
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
