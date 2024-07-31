using IdentityManagerServerApi.Models.DTOs;

namespace IdentityManagerServerApi.Data.Service
{
    public interface IUserAccount
    {
        Task<ServiceResponse.GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<ServiceResponse.LoginResponse> LoginAccount (LoginDTO loginDTO);
    }
}

