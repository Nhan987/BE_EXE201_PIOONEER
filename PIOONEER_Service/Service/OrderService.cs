using AutoMapper;
using Firebase.Auth;
using Microsoft.Extensions.Configuration;
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

        public OrderService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IEmailService email)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _email = email;
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
                var order = _unitOfWork.Orders.Get(filter: c => c.UserId == id && c.Status == "1").FirstOrDefault();

                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                var customerResponse = _mapper.Map<OrderResponse>(order);
                await _email.SendBillEmailAsync(customerResponse.UserId.ToString(), customerResponse);
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
                            listOrder = _unitOfWork.Orders.Get(filter: o => userIds.Contains(o.UserId)).ToList();
                        }
                    }

                    var orderResponse = _mapper.Map<IEnumerable<OrderResponse>>(listOrder);

                    // Gửi email nếu có tìm thấy người dùng và đơn đặt hàng (muốn gửi email thì bỏ cái này ra)
                    /*       if (users.Any() && orderResponse.Any())
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
                    */
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
                }
                var order = _mapper.Map<Order>(uo);
                order.UserId = customer.Id; 
                order.OrderCode = orderCode;
                order.Status = "processing";
                order.CreateDate = DateTime.Now;

                
                _unitOfWork.Orders.Insert(order);
                await _unitOfWork.SaveChangesAsync();

               
                var orderResponse = _mapper.Map<OrderResponse>(order);

                Console.WriteLine($"Sending email to: {customer.Email}");
                Console.WriteLine($"OrderResponse: {JsonConvert.SerializeObject(orderResponse)}");
                await _email.SendBillEmailAsync(customer.Email, orderResponse);
                Console.WriteLine("Email sent successfully.");

                return orderResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateOrder: {ex.Message}");
                throw;
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
