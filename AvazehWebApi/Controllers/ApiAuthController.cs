using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettings;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("Register")]
        public async Task<ActionResult<UserInfo>> Register(User_DTO_CreateUpdate user)
        {
            
        }
    }
}