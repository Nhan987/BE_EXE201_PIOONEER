using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface IUserService
    {
       
        IEnumerable<UserResponse> GetAllUsers(string searchQuery = null);
        Task<UserResponse> GetUserById(int id);
        Task<UserResponse> CreateUser(UserRequest userRequest);
        Task<UserResponse> UpdateUser(int id, UserRequest userRequest);
        Task<bool> DeleteUser(long id);
        Task<UserResponse> GetUserByEmail(string mail);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
