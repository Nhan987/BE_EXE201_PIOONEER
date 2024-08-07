﻿using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail);
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendBillEmailAsync(string toEmail, OrderResponse billResponse);

        Task<IEnumerable<OrderResponse>> SendListOrderEmailAsync(string toEmail, IEnumerable<OrderResponse> orders);
        Task<bool> VerifyOtpAsync(string email, string otp);
    }
}
