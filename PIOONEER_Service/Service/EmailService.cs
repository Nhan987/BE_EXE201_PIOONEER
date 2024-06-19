﻿using PIOONEER_Model.DTO;
using PIOONEER_Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PIOONEER_Repository.Repository;
using PIOONEER_Repository.Entity;
using System.Security.Cryptography;

namespace PIOONEER_Service.Service
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
 //       private readonly OtpManager _otpManager;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;

            _unitOfWork = unitOfWork;
        }

        public async Task SendBillEmailAsync(string toEmail, OrderResponse order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order cannot be null");
            }

            var subject = "Your Bill Information";

            var message = $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
        <h1 style='color: #007BFF;'>Thông tin đặt hàng</h1>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Yêu cầu đặt hàng:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.OrderRequirement}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Mã đặt hàng:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.OrderCode}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Phương thức thanh toán:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.PaymentMethod}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Create Date:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.CreateDate}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Total Price for 1 order:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.TotalPrice}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Status:</strong></td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.Status}</td>
            </tr>
        </table>
    </div>";




            // Gửi email
            await SendEmailAsync(toEmail, subject, message);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUsername"], _configuration["EmailSettings:SmtpPassword"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task SendListOrderEmailAsync(string toEmail, IEnumerable<OrderResponse> orders)
        {
            if (orders == null || !orders.Any())
            {
                throw new ArgumentNullException(nameof(orders), "Orders cannot be null or empty");
            }

            var subject = "Your Bill Information";

            var message = @"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
        <h1 style='color: #007BFF;'>Thông tin đặt hàng</h1>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <th style='padding: 8px; border: 1px solid #ddd;'>Yêu cầu đặt hàng</th>
                <th style='padding: 8px; border: 1px solid #ddd;'>Mã đặt hàng</th>
                <th style='padding: 8px; border: 1px solid #ddd;'>Phương thức thanh toán</th>
                <th style='padding: 8px; border: 1px solid #ddd;'>Create Date</th>
                <th style='padding: 8px; border: 1px solid #ddd;'>Total Price</th>
                <th style='padding: 8px; border: 1px solid #ddd;'>Status</th>
            </tr>";

            foreach (var order in orders)
            {
                message += $@"
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.OrderRequirement}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.OrderCode}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.PaymentMethod}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.CreateDate}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.TotalPrice}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{order.Status}</td>
            </tr>";
            }

            message += @"
        </table>
    </div>";

            // Gửi email
            await SendEmailAsync(toEmail, subject, message);
        }

        
        public async Task SendOtpEmailAsync(string toEmail)
        {
            var otp = GenerateOtp();
            await StoreOtpAsync(toEmail, otp);

            var subject = "Your OTP Code";
            var message = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h1 style='color: #007BFF;'>OTP Code</h1>
                    <p>Your OTP code is: <strong>{otp}</strong></p>
                    <p>This code will expire in 10 minutes.</p>
                </div>";

            await SendEmailAsync(toEmail, subject, message);
        }
        private string GenerateOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[4];
                rng.GetBytes(tokenData);
                return BitConverter.ToUInt32(tokenData, 0).ToString("D6");
            }
        }
        private async Task StoreOtpAsync(string email, string otp)
        {
            var otpEntity = new OtpEntity
            {
                Email = email,
                Otp = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };
             _unitOfWork.OtpRepository.AddAsync(otpEntity);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var otpEntity = _unitOfWork.OtpRepository
                .Get(filter: o => o.Email == email && o.Otp == otp)
                .FirstOrDefault();

            if (otpEntity != null && otpEntity.ExpiryTime > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}