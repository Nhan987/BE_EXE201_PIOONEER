using PIOONEER_Model.DTO;
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
        Task<OrderResponse> UpdateOrderBYID(int id, OrderUpDTO orderUpdateDTO);
        Task<OrderResponse> CreateUserOrder(userAndOrderDTO uo);
        Task<bool> DeleteOrder(int id);
        Task<OrderResponse> GetOrderByEmailUser(string mail);
    }
}
