﻿using IdentityManagerServerApi.Data;
using IdentityManagerServerApi.Data.Service;
using IdentityManagerServerApi.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<ServiceResponse.LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return new ServiceResponse.LoginResponse(false, null!, "Login Container is Empty");

            var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
            if(getUser is null)
                return new ServiceResponse.LoginResponse(false, null!, "User not found");

            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return new ServiceResponse.LoginResponse(false, null!, "Invalid email/password");

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);
            return new ServiceResponse.LoginResponse(true, token!, "Login succeeded");
            
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
