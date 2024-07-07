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
        public async Task<IEnumerable<OrderResponse>> SendListOrderEmailAsync(string toEmail, IEnumerable<OrderResponse> orders)
        {
            if (orders == null || !orders.Any())
            {
                throw new ArgumentNullException(nameof(orders), "Orders cannot be null or empty");
            }

            var subject = "Đơn hàng của bạn đã được tạo";
            var message = new StringBuilder();

            message.Append($@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Thông tin đơn hàng</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #ee4d2d;
            color: white;
            padding: 20px;
            text-align: center;
        }}
        .content {{
            padding: 20px;
            background-color: #f8f8f8;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }}
        th {{
            background-color: #ee4d2d;
            color: white;
        }}
        .total {{
            font-weight: bold;
            background-color: #f1f1f1;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Đơn hàng của bạn đã được tạo</h1>
    </div>
    <div class='content'>
        <p>Xin chào bạn,</p>
        <p>Dưới đây là thông tin về các đơn hàng của bạn:</p>");

            foreach (var order in orders)
            {
                message.Append($@"
        <h2>THÔNG TIN ĐƠN HÀNG - {order.OrderCode}</h2>
        <p><strong>Mã đơn hàng:</strong> {order.OrderCode}</p>
        <p><strong>Ngày đặt hàng:</strong> {order.CreateDate:dd/MM/yyyy HH:mm:ss}</p>
        <p><strong>Trạng thái đơn hàng:</strong> {order.Status}</p>
        <p><strong>Người bán:</strong> Piooneer Shop</p>
        
        <table>
            <tr>
                <th>Sản phẩm</th>
                <th style='text-align: right;'>Số lượng</th>
                <th style='text-align: right;'>Giá</th>
            </tr>");

                foreach (var item in order.OrderDetails)
                {
                    message.Append($@"
            <tr>
                <td>{item.ProductName}</td>
                <td style='text-align: right;'>{item.OrderQuantity}</td>
                <td style='text-align: right;'>{item.OrderPrice:N0}₫</td>
            </tr>");
                }

                message.Append($@"
            <tr class='total'>
                <td colspan='2' style='text-align: right;'>Tổng tiền hàng:</td>
                <td style='text-align: right;'>{order.TotalPrice:N0}₫</td>
            </tr>
            <tr>
                <td colspan='2' style='text-align: right;'>Phương thức vận chuyển:</td>
                <td style='text-align: right;'>{order.ShippingMethod}</td>
            </tr>
            <tr class='total'>
                <td colspan='2' style='text-align: right;'>Tổng thanh toán:</td>
                <td style='text-align: right;'>{order.TotalPrice:N0}₫</td>
            </tr>
        </table>
        
        <h3>BƯỚC TIẾP THEO</h3>
        <p>Bạn không hài lòng với sản phẩm? Hãy liên hệ với chúng tôi sớm nhất có thể</p>    
        <p>Chúc bạn luôn có những trải nghiệm tuyệt vời khi mua sắm tại Piooneer.</p>");
            }

            message.Append($@"
        <p>Trân trọng,<br>Đội ngũ Piooneer</p>
    </div>
    <div class='footer'>
        <p>Bạn có thắc mắc? <a href='#'>Liên hệ chúng tôi tại đây</a></p>
        <p>Hãy mua sắm cùng Piooneer</p>
    </div>
</body>
</html>");

            await SendEmailAsync(toEmail, subject, message.ToString());

            return orders;
        }


        private async Task<string> GetProductNameById(long productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            return product?.ProductName ?? "Unknown Product";
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
