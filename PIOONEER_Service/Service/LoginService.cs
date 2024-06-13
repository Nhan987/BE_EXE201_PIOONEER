using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
    public class LoginService:ILoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoginService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<(string Token, LoginResponse LoginResponse)> AuthorizeUser(LoginRequest loginRequest)
        {
            var customer = _unitOfWork.UserRepository
                .Get(filter: c => c.Email == loginRequest.Email && c.Status == "1").FirstOrDefault();

            if (customer != null && VerifyPassword(loginRequest.Password, customer.Password))
            {
                string token = GenerateToken(customer);
                var customerResponse = _mapper.Map<LoginResponse>(customer);
                return (token, customerResponse);
            }

            return (null, null);
        }
        private string GenerateToken(User info)
        {
            if (info == null || string.IsNullOrEmpty(info.Username))
            {
                throw new ArgumentNullException(nameof(info), "Customer information is null or invalid.");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", info.Id.ToString() ?? ""),
                new Claim("Email", info.Username ?? ""),
                new Claim("Name", info.Name ?? ""),
                new Claim("RoleId", info.RoleId.ToString() ?? ""),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            return EncryptPassword.Encrypt(providedPassword) == hashedPassword;
        }

        public string HashPassword(string password)
        {
            return EncryptPassword.Encrypt(password);
        }

    }
}
