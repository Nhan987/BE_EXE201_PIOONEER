using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using PIOONEER_Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace PIOONEER_Service.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }
    

        public IEnumerable<UserResponse> GetAllUsers(string searchQuery = null)
        {
            IEnumerable<User> listCustomers;

            if (string.IsNullOrEmpty(searchQuery))
            {
                listCustomers = _unitOfWork.UserRepository.Get(filter: c => c.Status == "1").ToList();
            }
            else
            {
                var loweredSearchQuery = searchQuery.ToLower();
                listCustomers = _unitOfWork.UserRepository.Get(filter: c => c.Status == "1" &&
                              (c.Name.ToLower().Contains(loweredSearchQuery) ||
                               c.Username.ToLower().Contains(loweredSearchQuery))).ToList();
            }

            var customerResponses = _mapper.Map<IEnumerable<UserResponse>>(listCustomers);
            return customerResponses;
        }

        public async Task<UserResponse> GetUserById(int id)
        {
            try
            {
                var customer = _unitOfWork.UserRepository.Get(filter: c => c.Id == id && c.Status == "1").FirstOrDefault();

                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }

                var customerResponse = _mapper.Map<UserResponse>(customer);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<UserResponse> GetUserByEmail(string mail)
        {
            try
            {
                var customer = _unitOfWork.UserRepository
                    .Get(filter: u => u.Email == mail && u.Status == "1")
                    .FirstOrDefault();

                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }

                var customerResponse = _mapper.Map<UserResponse>(customer);
                return customerResponse;
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logging mechanism
                throw new Exception($"An error occurred while retrieving the customer by email: {mail}", ex);
            }
        }

        public async Task<UserResponse> CreateUser(UserRequest userRequest)
        {
            try
            {
                var existingCustomer = _unitOfWork.UserRepository.Get(c => c.Email == userRequest.Email).FirstOrDefault();
                if (existingCustomer != null)
                {
                    throw new Exception("User with the same Email address already exists.");
                }

                var customer = _mapper.Map<User>(userRequest);
                customer.Password = HashPassword(userRequest.Password);
                customer.Status = "1";
                customer.RoleId = 2;
                _unitOfWork.UserRepository.Insert(customer);
                _unitOfWork.Save();

                var customerResponse = _mapper.Map<UserResponse>(customer);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserResponse> UpdateUser(int id, UserRequest userRequest)
        {
            try
            {
                var existingCustomer = _unitOfWork.UserRepository.GetByID(id);
                if (existingCustomer == null)
                {
                    throw new Exception("Customer not found.");
                }

                _mapper.Map(userRequest, existingCustomer);
                _unitOfWork.UserRepository.Update(existingCustomer);
                _unitOfWork.Save();

                var customerResponse = _mapper.Map<UserResponse>(existingCustomer);
                return customerResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var customer = _unitOfWork.UserRepository.GetByID(id);
                if (customer == null)
                {
                    throw new Exception("User not found.");
                }

                customer.Status = "0";
                _unitOfWork.UserRepository.Update(customer);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string HashPassword(string password)
        {
            return EncryptPassword.Encrypt(password);
        }
        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (await _emailService.VerifyOtpAsync(request.Email, request.Otp))
            {
                var user = _unitOfWork.UserRepository
                    .Get(filter: u => u.Email == request.Email && u.Status == "1")
                    .FirstOrDefault();

                if (user == null)
                {
                    return new ResetPasswordResponse { Message = "User not found." };
                }

                user.Password = HashPassword(request.NewPassword);
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return new ResetPasswordResponse { Message = "Password reset successfully." };
            }

            return new ResetPasswordResponse { Message = "Invalid OTP." };
        }

    }
}
