using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Service.Interface;
using Tools;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly IEmailService _email;

        public EmailController(IEmailService email)
        {
            _email = email;
        }
        [HttpPost("SendTestEmail")]
        public async Task<IActionResult> SendTestEmail([FromBody] EmailSendDTO emailSendDTO)
        {
            if (emailSendDTO == null)
            {
                return BadRequest("Invalid email request");
            }

            try
            {
                await _email.SendEmailAsync(emailSendDTO.To,emailSendDTO.Subject,emailSendDTO.message);
                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
