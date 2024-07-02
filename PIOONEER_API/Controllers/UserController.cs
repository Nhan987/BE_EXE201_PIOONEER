using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Service.Interface;
using System.Net;

namespace PIOONEER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

       
        [HttpGet]
        public IActionResult GetAllCustomer([FromQuery] string searchQuery = null)
        {
            var customers = _userService.GetAllUsers(searchQuery);
            return CustomResult("Data load successful", customers);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _userService.GetUserById(id);
                return CustomResult("Customer found", customer);
            }
            catch (Exception ex)
            {
                return CustomResult("Customer not found", HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }

            try
            {
                var result = await _userService.CreateUser(userRequest);
                return CustomResult("Create successful", result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User with the same Email address already exists."))
                {
                    return CustomResult(ex.Message, HttpStatusCode.Conflict);
                }
                return CustomResult("Create fail.", ex.Message, HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await _userService.UpdateUser(id, userRequest);
                return CustomResult("Update successful", result);
            }
            catch (Exception ex)
            {
                return CustomResult("Update customer fail", HttpStatusCode.BadRequest);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                if (result)
                {
                    return CustomResult("Delete successful.");
                }
                else
                {
                    return CustomResult("Customer not found.", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return CustomResult("Delete fail.", HttpStatusCode.BadRequest);
            }
        }
    }
}
