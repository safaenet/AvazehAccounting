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
        public AuthController(IUserProcessor userProcessor)
        {
            UserProcessor = userProcessor;
        }

        private readonly IUserProcessor UserProcessor;

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Register")]
        public async Task<ActionResult<bool>> Register(User_DTO_CreateUpdate user)
        {
            var newUser = await UserProcessor.CreateUser(user);
            if (newUser == null) return false; else return true;
        }

        [HttpPost("Login")]
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
            loggedUser.Token = GenerateToken(user.Username, userInfoBase, Permissions);
            loggedUser.Settings = Settings;
            loggedUser.DateCreated = userInfoBase.DateCreated;
            loggedUser.LastLoginDate = userInfoBase.LastLoginDate;
            return loggedUser;
        }

        [HttpPut("Update")]
        public async Task<ActionResult<bool>> UpdateUser(User_DTO_CreateUpdate user)
        {
            var updatedUser = await UserProcessor.UpdateUser(user);
            if (updatedUser == null) return false; else return true;
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteUser(string Username)
        {
            var result = await UserProcessor.DeleteUser(Username);
            if(result > 0) return true;
            return false;
        }

        private string GenerateToken(string Username, UserInfoBase userInfoBase, UserPermissions Permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Username),
                new Claim(ClaimTypes.Name, userInfoBase.FullName)
            };

            if (Permissions.CanViewCustomers) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewCustomers)));
            if (Permissions.CanViewProducts) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewProducts)));
            if (Permissions.CanViewInvoicesList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewInvoicesList)));
            if (Permissions.CanViewInvoiceDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewInvoiceDetails)));
            if (Permissions.CanViewTransactionsList) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewTransactionsList)));
            if (Permissions.CanViewTransactionDetails) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewTransactionDetails)));
            if (Permissions.CanViewCheques) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanViewCheques)));
            if (Permissions.CanAddNewCustomer) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddNewCustomer)));
            if (Permissions.CanAddNewProduct) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddNewProduct)));
            if (Permissions.CanAddNewInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddNewInvoice)));
            if (Permissions.CanAddNewTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddNewTransaction)));
            if (Permissions.CanAddNewCheque) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddNewCheque)));
            if (Permissions.CanEditCustomers) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditCustomers)));
            if (Permissions.CanEditProducts) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditProducts)));
            if (Permissions.CanEditInvoices) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditInvoices)));
            if (Permissions.CanEditTransactions) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditTransactions)));
            if (Permissions.CanEditCheques) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditCheques)));
            if (Permissions.CanDeleteCustomer) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteCustomer)));
            if (Permissions.CanDeleteProduct) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteProduct)));
            if (Permissions.CanDeleteInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteInvoice)));
            if (Permissions.CanDeleteInvoiceItem) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteInvoiceItem)));
            if (Permissions.CanDeleteTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteTransaction)));
            if (Permissions.CanDeleteTransactionItem) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteTransactionItem)));
            if (Permissions.CanDeleteCheque) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanDeleteCheque)));
            if (Permissions.CanPrintInvoice) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanPrintInvoice)));
            if (Permissions.CanPrintTransaction) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanPrintTransaction)));
            if (Permissions.CanChangeItsSettings) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanChangeItsSettings)));
            if (Permissions.CanChangeItsPassword) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanChangeItsPassword)));
            if (Permissions.CanAddUser) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanAddUser)));
            if (Permissions.CanEditOtherUsersPermission) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditOtherUsersPermission)));
            if (Permissions.CanEditOtherUsersSettings) claims.Add(new Claim(ClaimTypes.Role, nameof(UserPermissions.CanEditOtherUsersSettings)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SettingsDataAccess.AppConfiguration().GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}