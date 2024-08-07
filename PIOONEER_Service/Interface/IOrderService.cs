﻿using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface IOrderService
    {
        IEnumerable<OrderResponse> GetAllOrder(string searchQuery = null);
        Task<OrderResponse> GetOrderByID(int id);
        Task<OrderResponse> CreateOrder(OrderAddDTO orderAddDTO);
        Task<OrderUpdateResponse> UpdateOrderByCode(string orderCode, OrderUpDTO orderUp);
        
        Task<OrderResponse> CreateUserOrder(userAndOrderDTO uo);
        Task<OrderResponse> AssignOrderdetails(userAndOrderAndOrderdetailsDTO uo);
        Task<bool> DeleteOrder(int id);
        IEnumerable<OrderResponse> GetAllOrderByEmail(string searchQuery = null);
        IEnumerable<OrderResponse> GetAllOrderByEmailAndSendEmailAsync(string searchQuery = null);
    }
}
