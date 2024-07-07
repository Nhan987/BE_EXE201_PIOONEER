using AutoMapper;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Fpe;
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
using User = PIOONEER_Repository.Entity.User;

namespace PIOONEER_Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        DateTime CreateDate = DateTime.Now;
        private readonly MyDbContext _myDbContext;
        public OrderService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IEmailService email, MyDbContext myDbContext )
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _email = email;
            _myDbContext = myDbContext;
        }

        public IEnumerable<OrderResponse> GetAllOrder(string searchQuery = null)
        {
            IEnumerable<Order> query = _unitOfWork.Orders.Get(includeProperties: "OrderDetails");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var loweredSearchQuery = searchQuery.ToLower();
                query = query.Where(c => c.OrderCode.ToLower().Contains(loweredSearchQuery) ||
                                         c.TotalPrice.ToString().Equals(loweredSearchQuery));
            }

            var orderList = query.ToList();
            var orderResponses = new List<OrderResponse>();

            foreach (var order in orderList)
            {
                // Lấy thông tin User từ UserId của order
                var user = _unitOfWork.UserRepository.GetByID(order.UserId);
                if (user != null)
                {
                    // Tạo một đối tượng OrderResponse và map từ Order
                    var orderResponse = _mapper.Map<OrderResponse>(order);

                    // Lấy Username từ thuộc tính Username của đối tượng User
                    orderResponse.name = user.Name;

                    // Thêm OrderResponse vào danh sách
                    orderResponses.Add(orderResponse);
                }
            }

            return orderResponses;
        }

        public async Task<OrderResponse> GetOrderByID(int id)
        {
            try
            {
                var order = _unitOfWork.Orders.Get(filter: c => c.Id == id , includeProperties: "OrderDetails").FirstOrDefault();

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
        public IEnumerable<OrderResponse> GetAllOrderByEmail(string searchQuery = null)
        {
            IEnumerable<Order> listOrder = Enumerable.Empty<Order>();
            List<User> users = new List<User>();
            if (string.IsNullOrEmpty(searchQuery))
            {
                listOrder = _unitOfWork.Orders.Get(includeProperties: "OrderDetails,OrderDetails.Product").ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                users = _unitOfWork.UserRepository.Get(filter: c => c.Email.ToLower().Contains(loweredSearchQuery)).ToList();
                if (users.Any())
                {
                    var userIds = users.Select(u => u.Id).ToList();
                    listOrder = _unitOfWork.Orders.Get(
                        filter: o => userIds.Contains(o.UserId),
                        includeProperties: "OrderDetails,OrderDetails.Product"
                    ).ToList();
                }
            }

            var orderResponse = _mapper.Map<IEnumerable<OrderResponse>>(listOrder);

            // Manually map ProductName
            foreach (var order in orderResponse)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var orderDetail = listOrder
                        .SelectMany(o => o.OrderDetails)
                        .FirstOrDefault(od => od.Id == detail.Id);

                    if (orderDetail != null && orderDetail.Product != null)
                    {
                        detail.ProductName = orderDetail.Product.ProductName;  // Assuming Product has a Name property
                    }
                }
            }

            return orderResponse;
        }
        public IEnumerable<OrderResponse> GetAllOrderByEmailAndSendEmailAsync(string searchQuery = null)
        {
            IEnumerable<Order> listOrder = Enumerable.Empty<Order>();
            List<User> users = new List<User>();

            if (string.IsNullOrEmpty(searchQuery))
            {
                listOrder = _unitOfWork.Orders.Get(includeProperties: "OrderDetails,OrderDetails.Product").ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                users = _unitOfWork.UserRepository.Get(filter: c => c.Email.ToLower().Contains(loweredSearchQuery)).ToList();

                if (users.Any())
                {
                    var userIds = users.Select(u => u.Id).ToList();
                    listOrder = _unitOfWork.Orders.Get(
                        filter: o => userIds.Contains(o.UserId),
                        includeProperties: "OrderDetails,OrderDetails.Product"
                    ).ToList();
                }
            }

            var orderResponses = _mapper.Map<List<OrderResponse>>(listOrder);

            // Thêm ProductName vào OrderDetailsResponse
            foreach (var order in orderResponses)
            {
                var originalOrder = listOrder.FirstOrDefault(o => o.Id == order.Id);
                if (originalOrder != null)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var originalDetail = originalOrder.OrderDetails.FirstOrDefault(od => od.Id == detail.Id);
                        if (originalDetail?.Product != null)
                        {
                            detail.ProductName = originalDetail.Product.ProductName;
                        }
                    }
                }
            }

            // Gửi email nếu có tìm thấy người dùng và đơn đặt hàng
            if (users.Any() && orderResponses.Any())
            {
                foreach (var user in users)
                {
                    var userOrders = orderResponses.Where(o => o.name == user.Name).ToList();
                    if (userOrders.Any())
                    {
                        try
                        {
                            // Đảm bảo gọi phương thức SendListOrderEmailAsync một cách đồng bộ
                            _email.SendListOrderEmailAsync(user.Email, userOrders).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send email to {user.Email}: {ex.Message}");
                        }
                    }
                }
            }

            return orderResponses;
        }

        public static string GenerateRandomOrderCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<OrderResponse> CreateOrder(OrderAddDTO OrderAdd)
        {
            if (OrderAdd == null)
                throw new ArgumentNullException(nameof(OrderAdd));
            try
            {
                var orderCode = GenerateRandomOrderCode(7);

                var order = _mapper.Map<Order>(OrderAdd);
                order.OrderCode = orderCode;
                order.Status = "processing";
                order.CreateDate = DateTime.Now;  

                _unitOfWork.Orders.Insert(order);
                await _unitOfWork.SaveChangesAsync();


                var orderResponse = _mapper.Map<OrderResponse>(order);
                var user = _unitOfWork.UserRepository.Get(filter: c => c.Id == order.UserId).FirstOrDefault();

                await _email.SendBillEmailAsync(user.Email, orderResponse);

                return orderResponse;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in CreateOrder: {ex.Message}");
                throw;
            }
        }

public async Task<OrderResponse> CreateUserOrder(userAndOrderDTO uo)
{
    if (uo == null)
        throw new ArgumentNullException(nameof(uo));

    try
    {
        var orderCode = GenerateRandomOrderCode(7);

        var existingUser = _unitOfWork.UserRepository.Get(filter: c => c.Email == uo.Email).FirstOrDefault();

        User customer;

        if (existingUser == null)
        {
            customer = _mapper.Map<User>(uo);
            customer.Username = "";
            customer.Password = "";
            customer.RoleId = 2;
            customer.Status = "1";

            _unitOfWork.UserRepository.Insert(customer);
            await _unitOfWork.SaveChangesAsync();
            customer = _unitOfWork.UserRepository.Get(filter: c => c.Email == uo.Email).FirstOrDefault();
            if (customer == null || customer.Id == 0)
                throw new InvalidOperationException("User creation failed.");
        }
        else
        {
            customer = existingUser;
            customer.Address = uo.Address;
            customer.PhoneNumber = uo.PhoneNumber;
        }

        Order order;
        order = _mapper.Map<Order>(uo);
        order.UserId = customer.Id;
        order.OrderCode = orderCode;
        order.Status = "đang xử lí";
        order.CreateDate = DateTime.Now;

        _unitOfWork.Orders.Insert(order);
        await _unitOfWork.SaveChangesAsync();

        if (uo.OrderRequirement == "string")
        {
            order.OrderRequirement = " ";
        }

        double totalPrice = 0;
        var orderDetailsList = new List<OrderDetails>();

        if (uo.OrderDetail != null && uo.OrderDetail.Count > 0)
        {
            foreach (var detail in uo.OrderDetail)
            {
                var product = _unitOfWork.Products.Get(filter: p => p.ProductName == detail.ProductName).FirstOrDefault();
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with name {detail.ProductName} not found.");
                }

                var orderDetail = new OrderDetails
                {
                    ProductId = product.Id,
                    OrderPrice = detail.OrderPrice,
                    OrderQuantity = detail.OrderQuantity,
                    OrderId = order.Id
                };

                totalPrice += detail.OrderPrice * detail.OrderQuantity;

                _unitOfWork.OrderDetails.Insert(orderDetail);
                orderDetailsList.Add(orderDetail);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        order.TotalPrice = totalPrice;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        var orderResponse = _mapper.Map<OrderResponse>(order);
        
        // Map OrderDetails to OrderDetailsResponse, including ProductName
        orderResponse.OrderDetails = orderDetailsList.Select(od => new OrderDetailsResponse
        {
            Id = od.Id,
            ProductName = _unitOfWork.Products.GetByIdAsync(od.ProductId).Result?.ProductName,
            OrderId = od.OrderId,
            OrderQuantity = od.OrderQuantity,
            OrderPrice = od.OrderPrice
        }).ToList();

        // Wrap the single orderResponse in a collection
        var orderResponses = new List<OrderResponse> { orderResponse };

        // Send email notification
        await _email.SendListOrderEmailAsync(customer.Email, orderResponses);

        return orderResponse;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in CreateUserOrder: {ex.Message}");
        throw;
    }
}



        public async Task<OrderResponse> AssignOrderdetails(userAndOrderAndOrderdetailsDTO uo)
                {
                    if (uo == null)
                        throw new ArgumentNullException(nameof(uo));


                    var order = _unitOfWork.Orders.Get(filter: c => c.OrderCode == uo.OrderCode).FirstOrDefault();


                    if (order == null)
                    {
                        throw new InvalidOperationException("Order not found.");
                    }


                    var orderDetails = _mapper.Map<OrderDetails>(uo);
                    orderDetails.OrderId = order.Id;


                    _unitOfWork.OrderDetails.Insert(orderDetails);
                    await _unitOfWork.SaveChangesAsync();


                    var orderDetailsList = _unitOfWork.OrderDetails.Get(filter: od => od.OrderId == order.Id).ToList();


                    var orderResponse = _mapper.Map<OrderResponse>(order);
                    orderResponse.OrderDetails = _mapper.Map<ICollection<OrderDetailsResponse>>(orderDetailsList);

                    return orderResponse;
                }





        public async Task<OrderUpdateResponse> UpdateOrderByCode(string orderCode, OrderUpDTO orderUp)
        {
            try
            {
                var existingOrder = _unitOfWork.Orders.Get(o => o.OrderCode == orderCode, includeProperties: "OrderDetails")
                    .FirstOrDefault();

                if (existingOrder == null)
                {
                    throw new Exception("Order not found.");
                }

                // Cập nhật các trường nếu có giá trị mới (không null và không phải là chuỗi trắng)
                if (!string.IsNullOrWhiteSpace(orderUp.OrderRequirement) && orderUp.OrderRequirement != "string")
                {
                    existingOrder.OrderRequirement = orderUp.OrderRequirement;
                }
                if (!string.IsNullOrWhiteSpace(orderUp.shippingMethod) && orderUp.shippingMethod != "string")
                {
                    existingOrder.shippingMethod = orderUp.shippingMethod;
                }
                if (!string.IsNullOrWhiteSpace(orderUp.PaymentMethod) && orderUp.PaymentMethod != "string")
                {
                    existingOrder.PaymentMethod = orderUp.PaymentMethod;
                }
                if (!string.IsNullOrWhiteSpace(orderUp.Status) && orderUp.Status != "string")
                {
                    existingOrder.Status = orderUp.Status;
                }

                _unitOfWork.Orders.Update(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                var orderUpdateResponse = _mapper.Map<OrderUpdateResponse>(existingOrder);
                return orderUpdateResponse;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while updating the order.", ex);
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var order = _unitOfWork.Orders.Get(filter: c => c.Id == id && c.Status == "đang xử lí").FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                order.Status = "bị hủy";
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
