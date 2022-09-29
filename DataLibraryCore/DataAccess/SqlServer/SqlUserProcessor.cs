﻿using Dapper;
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

            INSERT INTO UserSettings (Username, ColorNewItem, ColorSoldItemColor, ColorNonSufficientFundItem, ColorCashedItem, ColorChequeNotification, ColorUpdatedItem,
            ColorBalancedItem, ColorDeptorItem, ColorCreditorItem, ColorInactiveItem, ColorArchiveItem, ColorDeletedItem, ColorNegativeProfit, ColorPositiveItem, ColorNegativeItem,
            DataGridFontSize, ChequeListPageSize, ChequeListQueryOrderType, ChequeNotifyDays, ChequeNotify, InvoicePageSize, InvoiceListQueryOrderType, InvoiceDetailQueryOrderType,
            TransactionListPageSize, TransactionDetailPageSize, TransactionListQueryOrderType, TransactionDetailQueryOrderType, AutoSelectPersianLanguage, TransactionShortcut1Id, TransactionShortcut2Id, TransactionShortcut3Id,
            TransactionShortcut1Name, TransactionShortcut2Name, TransactionShortcut3Name, AskToAddNotExistingProduct)
            
            VALUES (@username, @colorNewItem, @colorSoldItemColor, @colorNonSufficientFundItem, @colorCashedItem, @colorChequeNotification, @colorUpdatedItem,
            @colorBalancedItem, @colorDeptorItem, @colorCreditorItem, @colorInactiveItem, @colorArchiveItem, @colorDeletedItem, @colorNegativeProfit, @colorPositiveItem, @colorNegativeItem,
            @dataGridFontSize, @chequeListPageSize, @chequeListQueryOrderType, @chequeNotifyDays, @chequeNotify, @invoicePageSize, @invoiceListQueryOrderType, @invoiceDetailQueryOrderType,
            @transactionListPageSize, @transactionDetailPageSize, @transactionListQueryOrderType, @transactionDetailQueryOrderType, @autoSelectPersianLanguage, @transactionShortcut1Id, @transactionShortcut2Id, @transactionShortcut3Id,
            @transactionShortcut1Name, @transactionShortcut2Name, @transactionShortcut3Name, @askToAddNotExistingProduct);
            
            INSERT INTO UserInfo (Username, PasswordHash, PasswordSalt, FirstName, LastName, DateCreated)
            VALUES (@username, @passwordHash, @passwordSalt, @firstName, @lastName, @dateCreated);";

        private readonly string UpdateUserQuery = @"UPDATE UserInfo SET PasswordHash = @passwordHash, PasswordSalt = @passwordSalt, FirstName = @firstName, LastName = @lastName WHERE Username = @username;
            UPDATE UserPermissions SET CanViewCustomersList = @canViewCustomersList, CanViewCustomerDetails = @canViewCustomerDetails, CanViewProductsList = @canViewProductsList, CanViewProductDetails = @canViewProductDetails,
            CanViewInvoicesList = @canViewInvoicesList, CanViewInvoiceDetails = @canViewInvoiceDetails, CanViewTransactionsList = @canViewTransactionsList, CanViewTransactionDetails = @canViewTransactionDetails,
            CanViewChequesList = @canViewChequesList, CanViewChequeDetails = @canViewChequeDetails, CanAddNewCustomer = @canAddNewCustomer, CanAddNewProduct = @canAddNewProduct, CanAddNewInvoice = @canAddNewInvoice,
            CanAddNewTransaction = @canAddNewTransaction, CanAddNewCheque = @canAddNewCheque, CanEditCustomer = @canEditCustomer, CanEditProduct = @canEditProduct,
            CanEditInvoice = @canEditInvoice, CanEditTransaction = @canEditTransaction, CanEditCheque = @canEditCheque, CanDeleteCustomer = @canDeleteCustomer,
            CanDeleteProduct = @canDeleteProduct, CanDeleteInvoice = @canDeleteInvoice, CanDeleteInvoiceItem = @canDeleteInvoiceItem, CanDeleteTransaction = @canDeleteTransaction,
            CanDeleteTransactionItem = @canDeleteTransactionItem, CanDeleteCheque = @canDeleteCheque, CanPrintInvoice = @canPrintInvoice, CanPrintTransaction = @canPrintTransaction,
            CanViewNetProfits = @canViewNetProfits, CanUseBarcodeReader = @canUseBarcodeReader, CanManageItself = @canManageItself, CanManageOthers = @canManageOthers WHERE Username = @username;
            
            UPDATE UserSettings SET Username = @username, ColorNewItem = @colorNewItem, ColorSoldItemColor = @colorSoldItemColor, ColorNonSufficientFundItem = @colorNonSufficientFundItem,
            ColorCashedItem = @colorCashedItem, ColorChequeNotification = @colorChequeNotification, ColorUpdatedItem = @colorUpdatedItem, ColorBalancedItem = @colorBalancedItem,
            ColorDeptorItem = @colorDeptorItem, ColorCreditorItem = @colorCreditorItem, ColorInactiveItem = @colorInactiveItem, ColorArchiveItem = @colorArchiveItem,
            ColorDeletedItem = @colorDeletedItem, ColorNegativeProfit = @colorNegativeProfit, ColorPositiveItem = @colorPositiveItem, ColorNegativeItem = @colorNegativeItem,
            DataGridFontSize = @dataGridFontSize, ChequeListPageSize = @chequeListPageSize, ChequeListQueryOrderType = @chequeListQueryOrderType, ChequeNotifyDays = @chequeNotifyDays,
            ChequeNotify = @chequeNotify, InvoicePageSize = @invoicePageSize, InvoiceListQueryOrderType = @invoiceListQueryOrderType, InvoiceDetailQueryOrderType = @invoiceDetailQueryOrderType,
            TransactionListPageSize = @transactionListPageSize, TransactionDetailPageSize = @transactionDetailPageSize, TransactionListQueryOrderType = @transactionListQueryOrderType,
            TransactionDetailQueryOrderType = @transactionDetailQueryOrderType, AutoSelectPersianLanguage = @autoSelectPersianLanguage,
            TransactionShortcut1Id = @transactionShortcut1Id, TransactionShortcut2Id = @transactionShortcut2Id, TransactionShortcut3Id = transactionShortcut3Id,
            TransactionShortcut1Name = @transactionShortcut1Name, TransactionShortcut2Name = @transactionShortcut2Name, TransactionShortcut3Name = @transactionShortcut3Name,
            AskToAddNotExistingProduct = @askToAddNotExistingProduct WHERE Username = @username;";
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
            var dp = FillParameters(user);
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
            var dp = FillParameters(user);
            dp.Add("@passwordHash", newUser.PasswordHash);
            dp.Add("@passwordSalt", newUser.PasswordSalt);

            var AffectedCount = await DataAccess.SaveDataAsync(UpdateUserQuery, dp);
            if (AffectedCount > 0) return newUser; else return null;
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

        private DynamicParameters FillParameters(User_DTO_CreateUpdate user)
        {
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            dp.Add("@firstName", user.FirstName);
            dp.Add("@lastName", user.LastName);

            dp.Add("@canViewCustomersList", user.Permissions.CanViewCustomersList);
            dp.Add("@canViewCustomerDetails", user.Permissions.CanViewCustomerDetails);
            dp.Add("@canViewProductsList", user.Permissions.CanViewProductsList);
            dp.Add("@canViewProductDetails", user.Permissions.CanViewProductDetails);
            dp.Add("@canViewInvoicesList", user.Permissions.CanViewInvoicesList);
            dp.Add("@canViewInvoiceDetails", user.Permissions.CanViewInvoiceDetails);
            dp.Add("@canViewTransactionsList", user.Permissions.CanViewTransactionsList);
            dp.Add("@canViewTransactionDetails", user.Permissions.CanViewTransactionDetails);
            dp.Add("@canViewChequesList", user.Permissions.CanViewChequesList);
            dp.Add("@canViewChequeDetails", user.Permissions.CanViewChequeDetails);
            dp.Add("@canAddNewCustomer", user.Permissions.CanAddNewCustomer);
            dp.Add("@canAddNewProduct", user.Permissions.CanAddNewProduct);
            dp.Add("@canAddNewInvoice", user.Permissions.CanAddNewInvoice);
            dp.Add("@canAddNewTransaction", user.Permissions.CanAddNewTransaction);
            dp.Add("@canAddNewCheque", user.Permissions.CanAddNewCheque);
            dp.Add("@canEditCustomer", user.Permissions.CanEditCustomer);
            dp.Add("@canEditProduct", user.Permissions.CanEditProduct);
            dp.Add("@canEditInvoice", user.Permissions.CanEditInvoice);
            dp.Add("@canEditTransaction", user.Permissions.CanEditTransaction);
            dp.Add("@canEditCheque", user.Permissions.CanEditCheque);
            dp.Add("@canDeleteCustomer", user.Permissions.CanDeleteCustomer);
            dp.Add("@canDeleteProduct", user.Permissions.CanDeleteProduct);
            dp.Add("@canDeleteInvoice", user.Permissions.CanDeleteInvoice);
            dp.Add("@canDeleteInvoiceItem", user.Permissions.CanDeleteInvoiceItem);
            dp.Add("@canDeleteTransaction", user.Permissions.CanDeleteTransaction);
            dp.Add("@canDeleteTransactionItem", user.Permissions.CanDeleteTransactionItem);
            dp.Add("@canDeleteCheque", user.Permissions.CanDeleteCheque);
            dp.Add("@canPrintInvoice", user.Permissions.CanPrintInvoice);
            dp.Add("@canPrintTransaction", user.Permissions.CanPrintTransaction);
            dp.Add("@canViewNetProfits", user.Permissions.CanViewNetProfits);
            dp.Add("@canUseBarcodeReader", user.Permissions.CanUseBarcodeReader);
            dp.Add("@canManageItself", user.Permissions.CanManageItself);
            dp.Add("@canManageOthers", user.Permissions.CanManageOthers);

            dp.Add("@colorNewItem", user.Settings.ColorNewItem);
            dp.Add("@colorSoldItemColor", user.Settings.ColorSoldItemColor);
            dp.Add("@colorNonSufficientFundItem", user.Settings.ColorNonSufficientFundItem);
            dp.Add("@colorCashedItem", user.Settings.ColorCashedItem);
            dp.Add("@colorChequeNotification", user.Settings.ColorChequeNotification);
            dp.Add("@colorUpdatedItem", user.Settings.ColorUpdatedItem);
            dp.Add("@colorBalancedItem", user.Settings.ColorBalancedItem);
            dp.Add("@colorDeptorItem", user.Settings.ColorDeptorItem);
            dp.Add("@colorCreditorItem", user.Settings.ColorCreditorItem);
            dp.Add("@colorInactiveItem", user.Settings.ColorInactiveItem);
            dp.Add("@colorArchiveItem", user.Settings.ColorArchiveItem);
            dp.Add("@colorDeletedItem", user.Settings.ColorDeletedItem);
            dp.Add("@colorNegativeProfit", user.Settings.ColorNegativeProfit);
            dp.Add("@colorPositiveItem", user.Settings.ColorPositiveItem);
            dp.Add("@colorNegativeItem", user.Settings.ColorNegativeItem);
            dp.Add("@dataGridFontSize", user.Settings.DataGridFontSize);
            dp.Add("@chequeListPageSize", user.Settings.ChequeListPageSize);
            dp.Add("@chequeListQueryOrderType", user.Settings.ChequeListQueryOrderType);
            dp.Add("@chequeNotifyDays", user.Settings.ChequeNotifyDays);
            dp.Add("@chequeNotify", user.Settings.ChequeNotify);
            dp.Add("@invoicePageSize", user.Settings.InvoicePageSize);
            dp.Add("@invoiceListQueryOrderType", user.Settings.InvoiceListQueryOrderType);
            dp.Add("@invoiceDetailQueryOrderType", user.Settings.InvoiceDetailQueryOrderType);
            dp.Add("@transactionPageSize", user.Settings.TransactionListPageSize);
            dp.Add("@transactionDetailPageSize", user.Settings.TransactionDetailPageSize);
            dp.Add("@transactionListQueryOrderType", user.Settings.TransactionListQueryOrderType);
            dp.Add("@transactionDetailQueryOrderType", user.Settings.TransactionDetailQueryOrderType);
            dp.Add("@autoSelectPersianLanguage", user.Settings.AutoSelectPersianLanguage);
            dp.Add("@transactionShortcut1Id", user.Settings.TransactionShortcut1Id);
            dp.Add("@transactionShortcut2Id", user.Settings.TransactionShortcut2Id);
            dp.Add("@transactionShortcut3Id", user.Settings.TransactionShortcut3Id);
            dp.Add("@transactionShortcut1Name", user.Settings.TransactionShortcut1Name);
            dp.Add("@transactionShortcut2Name", user.Settings.TransactionShortcut2Name);
            dp.Add("@transactionShortcut3Name", user.Settings.TransactionShortcut3Name);
            dp.Add("@askToAddNotExistingProduct", user.Settings.AskToAddNotExistingProduct);

            return dp;
        }
    }
}