using Dapper;
using System.Linq;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System.Threading.Tasks;
using SharedLibrary.SecurityAndSettingsModels;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlUserProcessor : IUserProcessor
    {
        public SqlUserProcessor(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string CreateUserQuery = @"
            INSERT INTO UserPermissions (Username, CanViewCustomersList, CanViewCustomerDetails, CanViewProductsList, CanViewProductDetails, CanViewInvoicesList, CanViewInvoiceDetails, CanViewTransactionsList, CanViewTransactionDetails,
            CanViewChequesList, CanViewChequeDetails, CanAddNewCustomer, CanAddNewProduct, CanAddNewInvoice, CanAddNewTransaction, CanAddNewCheque, CanEditCustomer, CanEditProduct, CanEditInvoice,
            CanEditTransaction, CanEditCheque, CanDeleteCustomer, CanDeleteProduct, CanDeleteInvoice, CanDeleteInvoiceItem, CanDeleteTransaction, CanDeleteTransactionItem,
            CanDeleteCheque, CanPrintInvoice, CanPrintTransaction, CanViewNetProfits, CanUseBarcodeReader, CanManageItself, CanManageOthers)
            
            VALUES (@username, @canViewCustomersList, @canViewCustomerDetails, @canViewProductsList, @canViewProductDetails, @canViewInvoicesList, @canViewInvoiceDetails, @canViewTransactionsList, @canViewTransactionDetails,
            @canViewChequesList, @canViewChequeDetails, @canAddNewCustomer, @canAddNewProduct, @canAddNewInvoice, @canAddNewTransaction, @canAddNewCheque, @canEditCustomer, @canEditProduct, @canEditInvoice,
            @canEditTransaction, @canEditCheque, @canDeleteCustomer, @canDeleteProduct, @canDeleteInvoice, @canDeleteInvoiceItem, @canDeleteTransaction, @canDeleteTransactionItem,
            @canDeleteCheque, @canPrintInvoice, @canPrintTransaction, @canViewNetProfits, @canUseBarcodeReader, @canManageItself, @canManageOthers);

            INSERT INTO UserSettings (Username, ColorNewItem, ColorSoldItem, ColorNonSufficientFundItem, ColorCashedItem, ColorChequeNotification, ColorUpdatedItem,
            ColorBalancedItem, ColorDeptorItem, ColorCreditorItem, ColorInactiveItem, ColorArchivedItem, ColorDeletedItem, ColorNegativeProfit, ColorPositiveItem, ColorNegativeItem,
            DataGridFontSize, ChequeListPageSize, ChequeListQueryOrderType, ChequeNotifyDays, ChequeNotify, InvoicePageSize, InvoiceListQueryOrderType, InvoiceDetailQueryOrderType,
            TransactionListPageSize, TransactionDetailPageSize, TransactionListQueryOrderType, TransactionDetailQueryOrderType, AutoSelectPersianLanguage, TransactionShortcut1Id, TransactionShortcut2Id, TransactionShortcut3Id,
            TransactionShortcut1Name, TransactionShortcut2Name, TransactionShortcut3Name, AskToAddNotExistingProduct)
            
            VALUES (@username, @colorNewItem, @colorSoldItem, @colorNonSufficientFundItem, @colorCashedItem, @colorChequeNotification, @colorUpdatedItem,
            @colorBalancedItem, @colorDeptorItem, @colorCreditorItem, @colorInactiveItem, @colorArchivedItem, @colorDeletedItem, @colorNegativeProfit, @colorPositiveItem, @colorNegativeItem,
            @dataGridFontSize, @chequeListPageSize, @chequeListQueryOrderType, @chequeNotifyDays, @chequeNotify, @invoicePageSize, @invoiceListQueryOrderType, @invoiceDetailQueryOrderType,
            @transactionListPageSize, @transactionDetailPageSize, @transactionListQueryOrderType, @transactionDetailQueryOrderType, @autoSelectPersianLanguage, @transactionShortcut1Id, @transactionShortcut2Id, @transactionShortcut3Id,
            @transactionShortcut1Name, @transactionShortcut2Name, @transactionShortcut3Name, @askToAddNotExistingProduct);
            
            INSERT INTO UserInfo (Username, PasswordHash, PasswordSalt, FirstName, LastName, DateCreated)
            VALUES (@username, @passwordHash, @passwordSalt, @firstName, @lastName, @dateCreated);";

        private static readonly string UpdateUserInfoQuery = @"UPDATE UserInfo SET PasswordHash = @passwordHash, PasswordSalt = @passwordSalt, FirstName = @firstName, LastName = @lastName WHERE Username = @username;";
        private static readonly string UpdateUserPermissionsQuery = @"UPDATE UserPermissions SET CanViewCustomersList = @canViewCustomersList, CanViewCustomerDetails = @canViewCustomerDetails, CanViewProductsList = @canViewProductsList, CanViewProductDetails = @canViewProductDetails,
            CanViewInvoicesList = @canViewInvoicesList, CanViewInvoiceDetails = @canViewInvoiceDetails, CanViewTransactionsList = @canViewTransactionsList, CanViewTransactionDetails = @canViewTransactionDetails,
            CanViewChequesList = @canViewChequesList, CanViewChequeDetails = @canViewChequeDetails, CanAddNewCustomer = @canAddNewCustomer, CanAddNewProduct = @canAddNewProduct, CanAddNewInvoice = @canAddNewInvoice,
            CanAddNewTransaction = @canAddNewTransaction, CanAddNewCheque = @canAddNewCheque, CanEditCustomer = @canEditCustomer, CanEditProduct = @canEditProduct,
            CanEditInvoice = @canEditInvoice, CanEditTransaction = @canEditTransaction, CanEditCheque = @canEditCheque, CanDeleteCustomer = @canDeleteCustomer,
            CanDeleteProduct = @canDeleteProduct, CanDeleteInvoice = @canDeleteInvoice, CanDeleteInvoiceItem = @canDeleteInvoiceItem, CanDeleteTransaction = @canDeleteTransaction,
            CanDeleteTransactionItem = @canDeleteTransactionItem, CanDeleteCheque = @canDeleteCheque, CanPrintInvoice = @canPrintInvoice, CanPrintTransaction = @canPrintTransaction,
            CanViewNetProfits = @canViewNetProfits, CanUseBarcodeReader = @canUseBarcodeReader, CanManageItself = @canManageItself, CanManageOthers = @canManageOthers WHERE Username = @username;";
        private static readonly string UpdateUserSettingsQuery = @"UPDATE UserSettings SET Username = @username, ColorNewItem = @colorNewItem, ColorSoldItem = @colorSoldItem, ColorNonSufficientFundItem = @colorNonSufficientFundItem,
            ColorCashedItem = @colorCashedItem, ColorChequeNotification = @colorChequeNotification, ColorUpdatedItem = @colorUpdatedItem, ColorBalancedItem = @colorBalancedItem,
            ColorDeptorItem = @colorDeptorItem, ColorCreditorItem = @colorCreditorItem, ColorInactiveItem = @colorInactiveItem, ColorArchivedItem = @colorArchivedItem,
            ColorDeletedItem = @colorDeletedItem, ColorNegativeProfit = @colorNegativeProfit, ColorPositiveItem = @colorPositiveItem, ColorNegativeItem = @colorNegativeItem,
            DataGridFontSize = @dataGridFontSize, ChequeListPageSize = @chequeListPageSize, ChequeListQueryOrderType = @chequeListQueryOrderType, ChequeNotifyDays = @chequeNotifyDays,
            ChequeNotify = @chequeNotify, InvoicePageSize = @invoicePageSize, InvoiceListQueryOrderType = @invoiceListQueryOrderType, InvoiceDetailQueryOrderType = @invoiceDetailQueryOrderType,
            TransactionListPageSize = @transactionListPageSize, TransactionDetailPageSize = @transactionDetailPageSize, TransactionListQueryOrderType = @transactionListQueryOrderType,
            TransactionDetailQueryOrderType = @transactionDetailQueryOrderType, AutoSelectPersianLanguage = @autoSelectPersianLanguage,
            TransactionShortcut1Id = @transactionShortcut1Id, TransactionShortcut2Id = @transactionShortcut2Id, TransactionShortcut3Id = transactionShortcut3Id,
            TransactionShortcut1Name = @transactionShortcut1Name, TransactionShortcut2Name = @transactionShortcut2Name, TransactionShortcut3Name = @transactionShortcut3Name,
            AskToAddNotExistingProduct = @askToAddNotExistingProduct WHERE Username = @username;";
        private readonly string UpdateUserQuery = @$"{UpdateUserInfoQuery}{UpdateUserPermissionsQuery}{UpdateUserSettingsQuery}";
        private readonly string SelectUserInfoBase = @"SELECT Username, FirstName, LastName, DateCreated, LastLoginDate, LastLoginTime FROM UserInfo WHERE Username = @username";
        private readonly string SelectUserPermissions = @"SELECT * FROM UserPermissions WHERE Username = @username";
        private readonly string SelectUserSettings = @"SELECT * FROM UserSettings WHERE Username = @username";
        private readonly string GetPasswordHash = @"SELECT PasswordHash FROM UserInfo WHERE Username = @username";
        private readonly string GetPasswordSalt = @"SELECT PasswordSalt FROM UserInfo WHERE Username = @username";
        private readonly string DeleteUserFromDB = @"DELETE FROM UserInfo WHERE Username = @username; DELETE FROM UserPermissions WHERE Username = @username; DELETE FROM UserSettings WHERE Username = @username";
        private readonly string GetUsersListQuery = @"SELECT Username, FirstName, LastName, DateCreated, LastLoginDate, LastLoginTime FROM UserInfo";
        private readonly string GetCountOfAdminUsersQuery = @"SELECT COUNT(u.Username) FROM UserInfo u LEFT JOIN UserPermissions p ON u.Username = p.Username WHERE p.CanManageOthers = 1";
        private readonly string UpdateUserLastLoginDateQuery = @"UPDATE UserInfo SET LastLoginDate = @lastLoginDate, LastLoginTime = @lastLoginTime WHERE Username = @username";

        public async Task<List<UserInfoBaseModel>> GetUsersList()
        {
            var list =  await DataAccess.LoadDataAsync<UserInfoBaseModel, DynamicParameters>(GetUsersListQuery, null);
            return list.ToList();
        }

        public async Task<int> GetCountOfAdminUsers()
        {
            var count =  await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(GetCountOfAdminUsersQuery, null);
            return count;
        }

        public async Task<UserInfoBaseModel> CreateUser(User_DTO_CreateUpdate user)
        {
            if (user == null || user.Permissions == null || user.Settings == null) return null;
            CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            UserInfoBaseModel newUser = new()
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateCreated = PersianCalendarModel.GetCurrentPersianDate()
            };
            var dp = new DynamicParameters();
            FillUserBaseParameters(dp, user);
            FillUserPermissionParameters(dp, user.Permissions);
            FillUserSettingsParameters(dp, user.Settings);
            dp.Add("@passwordHash", PasswordHash, System.Data.DbType.Binary);
            dp.Add("@passwordSalt", PasswordSalt, System.Data.DbType.Binary);
            dp.Add("@dateCreated", newUser.DateCreated);

            var AffectedCount = await DataAccess.SaveDataAsync(CreateUserQuery, dp);
            if (AffectedCount > 0) return newUser; else return null;
        }

        public async Task<bool> VerifyUser(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return false;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var PasswordHash = await DataAccess.QuerySingleOrDefaultAsync<byte[], DynamicParameters>(GetPasswordHash, dp);
            var PasswordSalt = await DataAccess.QuerySingleOrDefaultAsync<byte[], DynamicParameters>(GetPasswordSalt, dp);
            if (PasswordHash == null || PasswordSalt == null) return false;
            return VerifyPasswordHash(user.Password, PasswordHash, PasswordSalt);
        }

        private bool VerifyPasswordHash(string password, byte[] oldPasswordHash, byte[] oldPasswordSalt)
        {
            using (var hmac = new HMACSHA512(oldPasswordSalt))
            {
                var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return ComputedHash.SequenceEqual(oldPasswordHash);
            }
        }

        public async Task<UserInfoBaseModel> GetUserInfoBase(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return null;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var userInfoBase = await DataAccess.QuerySingleOrDefaultAsync<UserInfoBaseModel, DynamicParameters>(SelectUserInfoBase, dp);
            return userInfoBase;
        }

        public async Task<UserPermissionsModel> GetUserPermissions(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return null;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var Permissions = await DataAccess.QuerySingleOrDefaultAsync<UserPermissionsModel, DynamicParameters>(SelectUserPermissions, dp);
            return Permissions;
        }

        public async Task<UserSettingsModel> GetUserSettings(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return null;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var Settings = await DataAccess.QuerySingleOrDefaultAsync<UserSettingsModel, DynamicParameters>(SelectUserSettings, dp);
            return Settings;
        }

        public async Task<UserInfoModel> UpdateUser(User_DTO_CreateUpdate user)
        {
            if (user == null || user.Permissions == null || user.Settings == null) return null;
            CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            UserInfoModel newUser = new()
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Permissions = user.Permissions,
                Settings = user.Settings,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt
            };
            var dp = new DynamicParameters();
            FillUserBaseParameters(dp, user);
            FillUserPermissionParameters(dp, user.Permissions);
            FillUserSettingsParameters(dp, user.Settings);
            dp.Add("@passwordHash", newUser.PasswordHash);
            dp.Add("@passwordSalt", newUser.PasswordSalt);

            var AffectedCount = await DataAccess.SaveDataAsync(UpdateUserQuery, dp);
            if (AffectedCount > 0) return newUser; else return null;
        }

        public async Task<bool> UpdateUserSettings(string Username, UserSettingsModel userSettings)
        {
            if (Username == null || userSettings == null) return false;
            var dp = new DynamicParameters();
            dp.Add("@username", Username);
            FillUserSettingsParameters(dp, userSettings);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateUserSettingsQuery, dp);
            return AffectedCount > 0;
        }

        public async Task<int> DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username)) return 0;
            DynamicParameters dp = new();
            dp.Add("@username", username);
            return await DataAccess.SaveDataAsync(DeleteUserFromDB, dp);
        }

        public async Task UpdateUserLastLoginDate(string username)
        {
            if (string.IsNullOrEmpty(username)) return;
            DynamicParameters dp = new();
            dp.Add("@username", username);
            dp.Add("@lastLoginDate", PersianCalendarModel.GetCurrentPersianDate());
            dp.Add("@lastLoginTime", PersianCalendarModel.GetCurrentTime());
            await DataAccess.SaveDataAsync(UpdateUserLastLoginDateQuery, dp);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private void FillUserBaseParameters(DynamicParameters dp, User_DTO_CreateUpdate user)
        {
            dp.Add("@username", user.Username);
            dp.Add("@firstName", user.FirstName);
            dp.Add("@lastName", user.LastName);
        }

        private void FillUserPermissionParameters(DynamicParameters dp, UserPermissionsModel Permissions)
        {
            dp.Add("@canViewCustomersList", Permissions.CanViewCustomersList);
            dp.Add("@canViewCustomerDetails", Permissions.CanViewCustomerDetails);
            dp.Add("@canViewProductsList", Permissions.CanViewProductsList);
            dp.Add("@canViewProductDetails", Permissions.CanViewProductDetails);
            dp.Add("@canViewInvoicesList", Permissions.CanViewInvoicesList);
            dp.Add("@canViewInvoiceDetails", Permissions.CanViewInvoiceDetails);
            dp.Add("@canViewTransactionsList", Permissions.CanViewTransactionsList);
            dp.Add("@canViewTransactionDetails", Permissions.CanViewTransactionDetails);
            dp.Add("@canViewChequesList", Permissions.CanViewChequesList);
            dp.Add("@canViewChequeDetails", Permissions.CanViewChequeDetails);
            dp.Add("@canAddNewCustomer", Permissions.CanAddNewCustomer);
            dp.Add("@canAddNewProduct", Permissions.CanAddNewProduct);
            dp.Add("@canAddNewInvoice", Permissions.CanAddNewInvoice);
            dp.Add("@canAddNewTransaction", Permissions.CanAddNewTransaction);
            dp.Add("@canAddNewCheque", Permissions.CanAddNewCheque);
            dp.Add("@canEditCustomer", Permissions.CanEditCustomer);
            dp.Add("@canEditProduct", Permissions.CanEditProduct);
            dp.Add("@canEditInvoice", Permissions.CanEditInvoice);
            dp.Add("@canEditTransaction", Permissions.CanEditTransaction);
            dp.Add("@canEditCheque", Permissions.CanEditCheque);
            dp.Add("@canDeleteCustomer", Permissions.CanDeleteCustomer);
            dp.Add("@canDeleteProduct", Permissions.CanDeleteProduct);
            dp.Add("@canDeleteInvoice", Permissions.CanDeleteInvoice);
            dp.Add("@canDeleteInvoiceItem", Permissions.CanDeleteInvoiceItem);
            dp.Add("@canDeleteTransaction", Permissions.CanDeleteTransaction);
            dp.Add("@canDeleteTransactionItem", Permissions.CanDeleteTransactionItem);
            dp.Add("@canDeleteCheque", Permissions.CanDeleteCheque);
            dp.Add("@canPrintInvoice", Permissions.CanPrintInvoice);
            dp.Add("@canPrintTransaction", Permissions.CanPrintTransaction);
            dp.Add("@canViewNetProfits", Permissions.CanViewNetProfits);
            dp.Add("@canUseBarcodeReader", Permissions.CanUseBarcodeReader);
            dp.Add("@canManageItself", Permissions.CanManageItself);
            dp.Add("@canManageOthers", Permissions.CanManageOthers);
        }

        private void FillUserSettingsParameters(DynamicParameters dp, UserSettingsModel Settings)
        {
            dp.Add("@colorNewItem", Settings.ColorNewItem);
            dp.Add("@colorSoldItem", Settings.ColorSoldItem);
            dp.Add("@colorNonSufficientFundItem", Settings.ColorNonSufficientFundItem);
            dp.Add("@colorCashedItem", Settings.ColorCashedItem);
            dp.Add("@colorChequeNotification", Settings.ColorChequeNotification);
            dp.Add("@colorUpdatedItem", Settings.ColorUpdatedItem);
            dp.Add("@colorBalancedItem", Settings.ColorBalancedItem);
            dp.Add("@colorDeptorItem", Settings.ColorDeptorItem);
            dp.Add("@colorCreditorItem", Settings.ColorCreditorItem);
            dp.Add("@colorInactiveItem", Settings.ColorInactiveItem);
            dp.Add("@colorArchivedItem", Settings.ColorArchivedItem);
            dp.Add("@colorDeletedItem", Settings.ColorDeletedItem);
            dp.Add("@colorNegativeProfit", Settings.ColorNegativeProfit);
            dp.Add("@colorPositiveItem", Settings.ColorPositiveItem);
            dp.Add("@colorNegativeItem", Settings.ColorNegativeItem);
            dp.Add("@dataGridFontSize", Settings.DataGridFontSize);
            dp.Add("@chequeListPageSize", Settings.ChequeListPageSize);
            dp.Add("@chequeListQueryOrderType", Settings.ChequeListQueryOrderType);
            dp.Add("@chequeNotifyDays", Settings.ChequeNotifyDays);
            dp.Add("@chequeNotify", Settings.ChequeNotify);
            dp.Add("@invoicePageSize", Settings.InvoicePageSize);
            dp.Add("@invoiceListQueryOrderType", Settings.InvoiceListQueryOrderType);
            dp.Add("@invoiceDetailQueryOrderType", Settings.InvoiceDetailQueryOrderType);
            dp.Add("@transactionListPageSize", Settings.TransactionListPageSize);
            dp.Add("@transactionDetailPageSize", Settings.TransactionDetailPageSize);
            dp.Add("@transactionListQueryOrderType", Settings.TransactionListQueryOrderType);
            dp.Add("@transactionDetailQueryOrderType", Settings.TransactionDetailQueryOrderType);
            dp.Add("@autoSelectPersianLanguage", Settings.AutoSelectPersianLanguage);
            dp.Add("@transactionShortcut1Id",Settings.TransactionShortcut1Id);
            dp.Add("@transactionShortcut2Id",Settings.TransactionShortcut2Id);
            dp.Add("@transactionShortcut3Id",Settings.TransactionShortcut3Id);
            dp.Add("@transactionShortcut1Name", Settings.TransactionShortcut1Name);
            dp.Add("@transactionShortcut2Name", Settings.TransactionShortcut2Name);
            dp.Add("@transactionShortcut3Name", Settings.TransactionShortcut3Name);
            dp.Add("@askToAddNotExistingProduct", Settings.AskToAddNotExistingProduct);
        }
    }
}