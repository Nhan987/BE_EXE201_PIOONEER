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
            var orderResponse = _mapper.Map<IEnumerable<OrderResponse>>(orderList);
            return orderResponse;
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
                    IEnumerable<Order> listOrder = Enumerable.Empty<Order>(); // Khởi tạo danh sách rỗng
                    List<User> users = new List<User>(); // Khởi tạo danh sách rỗng cho người dùng

                    if (string.IsNullOrEmpty(searchQuery))
                    {
                        listOrder = _unitOfWork.Orders.Get().ToList();
                    }


                    else
                    {
                        var loweredSearchQuery = searchQuery.ToLower();
                        users = _unitOfWork.UserRepository.Get(filter: c => c.Email.ToLower().Contains(loweredSearchQuery)).ToList();

                        // Tìm các đơn đặt hàng cho các người dùng tìm thấy
                        if (users.Any())
                        {
                            var userIds = users.Select(u => u.Id).ToList();
                            listOrder = _unitOfWork.Orders.Get(filter: o => userIds.Contains(o.UserId), includeProperties: "OrderDetails").ToList();
                        }
                    }

                    var orderResponse = _mapper.Map<IEnumerable<OrderResponse>>(listOrder);


                    return orderResponse;
                }
        public IEnumerable<OrderResponse> GetAllOrderByEmailButCanSendEmail(string searchQuery = null)
        {
            IEnumerable<Order> listOrder = Enumerable.Empty<Order>(); // Khởi tạo danh sách rỗng
            List<User> users = new List<User>(); // Khởi tạo danh sách rỗng cho người dùng

            if (string.IsNullOrEmpty(searchQuery))
            {
                listOrder = _unitOfWork.Orders.Get().ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                users = _unitOfWork.UserRepository.Get(filter: c => c.Email.ToLower().Contains(loweredSearchQuery)).ToList();

                // Tìm các đơn đặt hàng cho các người dùng tìm thấy
                if (users.Any())
                {
                    var userIds = users.Select(u => u.Id).ToList();
                    listOrder = _unitOfWork.Orders.Get(filter: o => userIds.Contains(o.UserId)).ToList();
                }
            }

            var orderResponse = _mapper.Map<IEnumerable<OrderResponse>>(listOrder);

            // Gửi email nếu có tìm thấy người dùng và đơn đặt hàng (muốn gửi email thì bỏ cái này ra)
                  if (users.Any() && orderResponse.Any())
                     {
                         foreach (var user in users)
                         {
                             var userOrders = orderResponse.Where(o => o.UserId == user.Id).ToList();
                             if (userOrders.Any())
                             {
                                 try
                                 {
                                     _ = _email.SendListOrderEmailAsync(user.Email, userOrders);
                                 }
                                 catch (Exception ex)
                                 {

                                 }
                             }
                         }
                     }
          
            return orderResponse;
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

                        _unitOfWork.OrderDetails.Insert(orderDetail);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                var orderDetailsList = _unitOfWork.OrderDetails.Get(filter: od => od.OrderId == order.Id).ToList();
                var orderResponse = _mapper.Map<OrderResponse>(order);
                orderResponse.OrderDetails = _mapper.Map<ICollection<OrderDetailsResponse>>(orderDetailsList);

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
