using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using DataLibraryCore.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        public AuthController(IUserProcessor userProcessor, IAppSettingsManager appSettingsManager)
        {
            UserProcessor = userProcessor;
            ASM = appSettingsManager;
        }

        private readonly IUserProcessor UserProcessor;
        private readonly IAppSettingsManager ASM;

        [HttpPost("Register"), AllowAnonymous]
        public async Task<ActionResult<UserInfoBaseModel>> Register(User_DTO_CreateUpdate user)
        {
            var adminsCount = await UserProcessor.GetCountOfAdminUsers();
            if (adminsCount > 0 && !User.IsInRole(nameof(UserPermissionsModel.CanManageOthers))) return BadRequest("اجازه صادر نشد");
            var newUser = await UserProcessor.CreateUser(user);
            return newUser;
        }

        [HttpPost("Login"), AllowAnonymous]
        public async Task<ActionResult<LoggedInUser_DTO>> Login(UserLogin_DTO user)
        {
            var IsVerified = await UserProcessor.VerifyUser(user);
            if (!IsVerified) return BadRequest("نام کاربری یا رمز عبور اشتباه است");
            var userInfoBase = await UserProcessor.GetUserInfoBase(user);
            if (userInfoBase == null) return BadRequest("مشخصات کاربر یافت نشد");
            var Permissions = await UserProcessor.GetUserPermissions(user);
            if (Permissions == null) return BadRequest("مجوز های کاربر یافت نشد");
            var Settings = await UserProcessor.GetUserSettings(user);
            if (Settings == null) return BadRequest("تنظیمات کاربر یافت نشد");

            //UserInfoBase userInfoBase = new();
            //userInfoBase.FirstName = "Safa";
            //userInfoBase.LastName = "Dana";
            //userInfoBase.DateCreated = "1404/04/04";
            //UserPermissions Permissions = new();
            //UserSettings Settings = new();

            LoggedInUser_DTO loggedUser = new();
            loggedUser.Username = user.Username;
            loggedUser.Token = GenerateToken(userInfoBase, Permissions);
            loggedUser.UserSettings = Settings;
            loggedUser.DateCreated = userInfoBase.DateCreated;
            loggedUser.LastLoginDate = userInfoBase.LastLoginDate;

            var appSettings = await ASM.LoadAllSettingsAsync();
            loggedUser.GeneralSettings = appSettings.GeneralSettings;
            loggedUser.PrintSettings = appSettings.PrintSettings;
            await UserProcessor.UpdateUserLastLoginDate(user.Username);
            return loggedUser;
        }

        [HttpGet("AdminExists"), AllowAnonymous]
        public async Task<ActionResult<bool>> AdminExists()
        {
            var adminsCount = await UserProcessor.GetCountOfAdminUsers();
            return adminsCount > 0;
        }

        [HttpPut("Update"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanManageOthers))]
        public async Task<ActionResult<bool>> UpdateUser(User_DTO_CreateUpdate user)
        {
            var updatedUser = await UserProcessor.UpdateUser(user);
            if (updatedUser == null) return false; else return true;
        }

        [HttpPut("UpdateUserSettings"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanManageItself))]
        public async Task<ActionResult<bool>> UpdateUserSettings(User_DTO_CreateUpdate user)
        {
            var result = await UserProcessor.UpdateUserSettings(user.Username, user.Settings);
            return result;
        }

        [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(UserPermissionsModel.CanManageItself)}, {nameof(UserPermissionsModel.CanManageOthers)}")]
        public async Task<ActionResult<bool>> DeleteUser(string Username)
        {
            var ActorUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ActorUser == Username) return BadRequest("اجازه حذف خودتان را ندارید!");
            if (ActorUser != Username && !User.IsInRole(nameof(UserPermissionsModel.CanManageOthers))) return BadRequest("اجازه صادر نشد");
            var result = await UserProcessor.DeleteUser(Username);
            if(result > 0) return true;
            return false;
        }

        private string GenerateToken(UserInfoBaseModel userInfoBase, UserPermissionsModel Permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfoBase.Username),
                new Claim(ClaimTypes.Name, userInfoBase.FullName)
            };

            if (Permissions.CanViewCustomersList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewCustomersList)));
            if (Permissions.CanViewCustomerDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewCustomerDetails)));
            if (Permissions.CanViewProductsList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewProductsList)));
            if (Permissions.CanViewProductDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewProductDetails)));
            if (Permissions.CanViewInvoicesList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewInvoicesList)));
            if (Permissions.CanViewInvoiceDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewInvoiceDetails)));
            if (Permissions.CanViewTransactionsList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewTransactionsList)));
            if (Permissions.CanViewTransactionDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewTransactionDetails)));
            if (Permissions.CanViewChequesList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewChequesList)));
            if (Permissions.CanViewChequeDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewChequeDetails)));
            if (Permissions.CanAddNewCustomer) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanAddNewCustomer)));
            if (Permissions.CanAddNewProduct) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanAddNewProduct)));
            if (Permissions.CanAddNewInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanAddNewInvoice)));
            if (Permissions.CanAddNewTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanAddNewTransaction)));
            if (Permissions.CanAddNewCheque) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanAddNewCheque)));
            if (Permissions.CanEditCustomer) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanEditCustomer)));
            if (Permissions.CanEditProduct) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanEditProduct)));
            if (Permissions.CanEditInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanEditInvoice)));
            if (Permissions.CanEditTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanEditTransaction)));
            if (Permissions.CanEditCheque) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanEditCheque)));
            if (Permissions.CanDeleteCustomer) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteCustomer)));
            if (Permissions.CanDeleteProduct) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteProduct)));
            if (Permissions.CanDeleteInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteInvoice)));
            if (Permissions.CanDeleteInvoiceItem) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteInvoiceItem)));
            if (Permissions.CanDeleteTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteTransaction)));
            if (Permissions.CanDeleteTransactionItem) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteTransactionItem)));
            if (Permissions.CanDeleteCheque) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanDeleteCheque)));
            if (Permissions.CanPrintInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanPrintInvoice)));
            if (Permissions.CanPrintTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanPrintTransaction)));
            if (Permissions.CanViewNetProfits) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanViewNetProfits)));
            if (Permissions.CanUseBarcodeReader) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanUseBarcodeReader)));
            if (Permissions.CanManageItself) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanManageItself)));
            if (Permissions.CanManageOthers) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissionsModel.CanManageOthers)));

            var validHours = SettingsDataAccess.AppConfiguration().GetSection("Jwt:ValidHours").Value;
            double.TryParse(validHours, out var addHours);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SettingsDataAccess.AppConfiguration().GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(addHours),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}