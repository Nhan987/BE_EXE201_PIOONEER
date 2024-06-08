using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface IOrderDetailService
    {
        IEnumerable<OrderDetailsResponse> GetAllOrderDetail();
        Task<OrderDetailsResponse> GetOrderDetailByID(int id);
        Task<OrderDetailsResponse> CreateOrderDetail(OrderDetailsAddDTO OrderDetailsAddDTO);
        Task<OrderDetailsResponse> UpdateOrderDetailBYID(int id,OrderDetailsUpDTO OrderUpdateDTO);
        Task<bool> DeleteOrderDetail(int id);
    }
}
