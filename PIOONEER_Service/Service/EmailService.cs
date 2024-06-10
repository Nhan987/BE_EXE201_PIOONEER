using PIOONEER_Model.DTO;
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

        public Task SendOtpEmailAsync(string toEmail)
        {
            throw new NotImplementedException();
        }
    }
}
