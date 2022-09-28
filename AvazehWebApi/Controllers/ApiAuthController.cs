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

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                new Claim(ClaimTypes.Name, userInfoBase.FullName),

                new Claim(nameof(UserPermissions.CanViewCustomers), Permissions.CanViewCustomers.ToString()),
                new Claim(nameof(UserPermissions.CanViewProducts), Permissions.CanViewProducts.ToString()),
                new Claim(nameof(UserPermissions.CanViewInvoicesList), Permissions.CanViewInvoicesList.ToString()),
                new Claim(nameof(UserPermissions.CanViewInvoiceDetails), Permissions.CanViewInvoiceDetails.ToString()),
                new Claim(nameof(UserPermissions.CanViewTransactionsList), Permissions.CanViewTransactionsList.ToString()),
                new Claim(nameof(UserPermissions.CanViewTransactionDetails), Permissions.CanViewTransactionDetails.ToString()),
                new Claim(nameof(UserPermissions.CanViewCheques), Permissions.CanViewCheques.ToString()),
                new Claim(nameof(UserPermissions.CanAddNewCustomer), Permissions.CanAddNewCustomer.ToString()),
                new Claim(nameof(UserPermissions.CanAddNewProduct), Permissions.CanAddNewProduct.ToString()),
                new Claim(nameof(UserPermissions.CanAddNewInvoice), Permissions.CanAddNewInvoice.ToString()),
                new Claim(nameof(UserPermissions.CanAddNewTransaction), Permissions.CanAddNewTransaction.ToString()),
                new Claim(nameof(UserPermissions.CanAddNewCheque), Permissions.CanAddNewCheque.ToString()),
                new Claim(nameof(UserPermissions.CanEditCustomers), Permissions.CanEditCustomers.ToString()),
                new Claim(nameof(UserPermissions.CanEditProducts), Permissions.CanEditProducts.ToString()),
                new Claim(nameof(UserPermissions.CanEditInvoices), Permissions.CanEditInvoices.ToString()),
                new Claim(nameof(UserPermissions.CanEditTransactions), Permissions.CanEditTransactions.ToString()),
                new Claim(nameof(UserPermissions.CanEditCheques), Permissions.CanEditCheques.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteCustomer), Permissions.CanDeleteCustomer.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteProduct), Permissions.CanDeleteProduct.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteInvoice), Permissions.CanDeleteInvoice.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteInvoiceItem), Permissions.CanDeleteInvoiceItem.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteTransaction), Permissions.CanDeleteTransaction.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteTransactionItem), Permissions.CanDeleteTransactionItem.ToString()),
                new Claim(nameof(UserPermissions.CanDeleteCheque), Permissions.CanDeleteCheque.ToString()),
                new Claim(nameof(UserPermissions.CanPrintInvoice), Permissions.CanPrintInvoice.ToString()),
                new Claim(nameof(UserPermissions.CanPrintTransaction), Permissions.CanPrintTransaction.ToString()),
                new Claim(nameof(UserPermissions.CanChangeItsSettings), Permissions.CanChangeItsSettings.ToString()),
                new Claim(nameof(UserPermissions.CanChangeItsPassword), Permissions.CanChangeItsPassword.ToString()),
                new Claim(nameof(UserPermissions.CanAddUser), Permissions.CanAddUser.ToString()),
                new Claim(nameof(UserPermissions.CanEditOtherUsersPermission), Permissions.CanEditOtherUsersPermission.ToString()),
                new Claim(nameof(UserPermissions.CanEditOtherUsersSettings), Permissions.CanEditOtherUsersSettings.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SettingsDataAccess.AppConfiguration().GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}