using IdentityManagerServerApi.Data;
using IdentityManagerServerApi.Data.Service;
using IdentityManagerServerApi.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityManagerServerApi.Repositories
{
    public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserAccount
    {
        public async Task<ServiceResponse.GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            if(userDTO is null) return new ServiceResponse.GeneralResponse(false, "Modal is empty");

            var newUser = new ApplicationUser()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                UserName = userDTO.Email,
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return new ServiceResponse.GeneralResponse(false, "user registered already");

            var createUser = await userManager.CreateAsync(newUser!, userDTO.Password);
            if (!createUser.Succeeded) return new ServiceResponse.GeneralResponse(false, "error occured.. please try again later");

            //Assign Default Role : admin to first registrar; rest is user
            var checkAdmin = await roleManager.FindByNameAsync("Admin");

            if (checkAdmin is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new ServiceResponse.GeneralResponse(true, "Account created");
            }
            else {
                var checkUser = await roleManager.FindByNameAsync("User");
                if (checkUser is null) await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

                await userManager.AddToRoleAsync(newUser, "User");
                return new ServiceResponse.GeneralResponse(true, "User account created");
            }

        }

        public Task<ServiceResponse.LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }
    }
}
