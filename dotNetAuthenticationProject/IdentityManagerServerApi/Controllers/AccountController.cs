using IdentityManagerServerApi.Data.Service;
using IdentityManagerServerApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagerServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase //methandi attamtama krla thiynne constructor ekk hdla thiyna ke me widyt class name ekt warahan daala constructor ekk hdna eka attama lesi
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response = await userAccount.CreateAccount(userDTO);
            return Ok(response);
        }

        //meka test kraddi hodat methk thiygnna password eka widiyt password ekk deddi capital akuru ekka special symbol ekk ena wiiyt denna nattnm iodentity user ekn reject wenwa 

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            return Ok(response);
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public IActionResult sayHello()
        {
            return Ok("hello world this site only can be accessed with a user account");
        }
    }
}
