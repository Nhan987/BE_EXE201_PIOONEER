using PIOONEER_Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Service.Interface
{
    public interface ILoginService
    {
        Task<(string Token, LoginResponse LoginResponse)> AuthorizeUser(LoginRequest loginRequest);
    }
}
