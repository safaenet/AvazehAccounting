using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        public AuthController(IAuthProcessor authProcessor)
        {
            AuthProcessor = authProcessor;
        }

        private readonly IAuthProcessor AuthProcessor;

        [HttpPost("Register")]
        public async Task<ActionResult<bool>> Register(User_DTO_CreateUpdate user)
        {
            var newUser = await AuthProcessor.CreateUser(user);
            if (newUser == null) return false; else return true;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLogin_DTO user)
        {
            //var IsVerified = await AuthProcessor.VerifyUser(user);
            //if (!IsVerified) return BadRequest("نام کاربری یا رمز عبور اشتباه است");
            var t= await AuthProcessor.GetUserByCredencials(user);
            return t;
        }
    }
}