using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.ObjectModel;
using FluentValidation.Results;
using DataLibraryCore.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.Validators;
using SharedLibrary.Enums;
using DataLibraryCore.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharedLibrary.DtoModels;
using SharedLibrary.SecurityAndSettingsModels;
using System.Security.Cryptography;
using System.Text;

namespace DataLibraryCore.DataAccess.SqlServer
{
    public class SqlAuthProcessor
    {
        public SqlAuthProcessor(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        private readonly IDataAccess DataAccess;
        private readonly string CreateUserQuery = @"INSERT INTO UserInfo (Username, PasswordHash, PasswordSalt, FirstName, LastName, DateCreated)
            VALUES (@username, @passwordHash, @passwordSalt, @firstName, @lastName, @dateCreated);
            
            INSERT INTO UserPermissions (Username, CanViewCustomers, CanViewProducts, CanViewInvoicesList, CanViewInvoiceDetails, CanViewTransactionsList, CanViewTransactionDetails,
            CanViewCheques, CanAddNewCustomer, CanAddNewProduct, CanAddNewInvoice, CanAddNewTransaction, CanAddNewCheque, CanEditCustomers, CanEditProducts, CanEditInvoices,
            CanEditTransactions, CanEditCheques, CanDeleteCustomer, CanDeleteProduct, CanDeleteInvoice, CanDeleteInvoiceItem, CanDeleteTransaction, CanDeleteTransactionItem,
            CanDeleteCheque, CanPrintInvoice, CanPrintTransaction, CanChangeItsSettings, CanChangeItsPassword, CanAddUser, CanEditOtherUsersPermission, CanEditOtherUsersSettings)
            
            VALUES (@username, @canViewCustomers, @canViewProducts, @canViewInvoicesList, @canViewInvoiceDetails, @canViewTransactionsList, @canViewTransactionDetails,
            @canViewCheques, @canAddNewCustomer, @canAddNewProduct, @canAddNewInvoice, @canAddNewTransaction, @canAddNewCheque, @canEditCustomers, @canEditProducts, @canEditInvoices,
            @canEditTransactions, @canEditCheques, @canDeleteCustomer, @canDeleteProduct, @canDeleteInvoice, @canDeleteInvoiceItem, @canDeleteTransaction, @canDeleteTransactionItem,
            @canDeleteCheque, @canPrintInvoice, @canPrintTransaction, @canChangeItsSettings, @canChangeItsPassword, @canAddUser, @canEditOtherUsersPermission, @canEditOtherUsersSettings);
            
            INSERT INTO UserSettings (Username, ColorNewItem, ColorSoldItemColor, ColorNonSufficientFundItem, ColorCashedItem, ColorChequeNotification, ColorUpdatedItem,
            ColorBalancedItem, ColorDeptorItem, ColorCreditorItem, ColorInactiveItem, ColorArchiveItem, ColorDeletedItem, ColorNegativeProfit, ColorPositiveItem, ColorNegativeItem,
            DataGridFontSize, ChequeListPageSize, ChequeListQueryOrderType, ChequeNotifyDays, ChequeNotify, InvoicePageSize, InvoiceListQueryOrderType, InvoiceDetailQueryOrderType,
            TransactionPageSize, TransactionListQueryOrderType, AutoSelectPersianLanguage, TransactionShortcut1Id, TransactionShortcut2Id, TransactionShortcut3Id,
            TransactionShortcut1Name, TransactionShortcut2Name, TransactionShortcut3Name, AskToAddNotExistingProduct, CanViewNetProfits, CanUseBarcodeReader)
            
            VALUES (@username, @colorNewItem, @colorSoldItemColor, @colorNonSufficientFundItem, @colorCashedItem, @colorChequeNotification, @colorUpdatedItem,
            @colorBalancedItem, @colorDeptorItem, @colorCreditorItem, @colorInactiveItem, @colorArchiveItem, @colorDeletedItem, @colorNegativeProfit, @colorPositiveItem, @colorNegativeItem,
            @dataGridFontSize, @chequeListPageSize, @chequeListQueryOrderType, @chequeNotifyDays, @chequeNotify, @invoicePageSize, @invoiceListQueryOrderType, @invoiceDetailQueryOrderType,
            @transactionPageSize, @transactionListQueryOrderType, @autoSelectPersianLanguage, @transactionShortcut1Id, @transactionShortcut2Id, @transactionShortcut3Id,
            @transactionShortcut1Name, @transactionShortcut2Name, @transactionShortcut3Name, @askToAddNotExistingProduct, @canViewNetProfits, @canUseBarcodeReader);";
        private readonly string UpdateUserQuery = @"UPDATE UserInfo SET PasswordHash = @passwordHash, PasswordSalt = @passwordSalt, FirstName = @firstName, LastName = @lastName WHERE Username = @username;
            UPDATE UserPermissions SET CanViewCustomers = @canViewCustomers, CanViewProducts = @canViewProducts, CanViewInvoicesList = @canViewInvoicesList,
            CanViewInvoiceDetails = @canViewInvoiceDetails, CanViewTransactionsList = @canViewTransactionsList, CanViewTransactionDetails = @canViewTransactionDetails,
            CanViewCheques = @canViewCheques, CanAddNewCustomer = @canAddNewCustomer, CanAddNewProduct = @canAddNewProduct, CanAddNewInvoice = @canAddNewInvoice,
            CanAddNewTransaction = @canAddNewTransaction, CanAddNewCheque = @canAddNewCheque, CanEditCustomers = @canEditCustomers, CanEditProducts = @canEditProducts,
            CanEditInvoices = @canEditInvoices, CanEditTransactions = @canEditTransactions, CanEditCheques = @canEditCheques, CanDeleteCustomer = @canDeleteCustomer,
            CanDeleteProduct = @canDeleteProduct, CanDeleteInvoice = @canDeleteInvoice, CanDeleteInvoiceItem = @canDeleteInvoiceItem, CanDeleteTransaction = @canDeleteTransaction,
            CanDeleteTransactionItem = @canDeleteTransactionItem, CanDeleteCheque = @canDeleteCheque, CanPrintInvoice = @canPrintInvoice, CanPrintTransaction = @canPrintTransaction,
            CanChangeItsSettings = @canChangeItsSettings, CanChangeItsPassword = @canChangeItsPassword, CanAddUser = @canAddUser, CanEditOtherUsersPermission = @canEditOtherUsersPermission,
            CanEditOtherUsersSettings = @canEditOtherUsersSettings WHERE Username = @username;
            
            UPDATE UserSettings SET Username = @username, ColorNewItem = @colorNewItem, ColorSoldItemColor = @colorSoldItemColor, ColorNonSufficientFundItem = @colorNonSufficientFundItem,
            ColorCashedItem = @colorCashedItem, ColorChequeNotification = @colorChequeNotification, ColorUpdatedItem = @colorUpdatedItem, ColorBalancedItem = @colorBalancedItem,
            ColorDeptorItem = @colorDeptorItem, ColorCreditorItem = @colorCreditorItem, ColorInactiveItem = @colorInactiveItem, ColorArchiveItem = @colorArchiveItem,
            ColorDeletedItem = @colorDeletedItem, ColorNegativeProfit = @colorNegativeProfit, ColorPositiveItem = @colorPositiveItem, ColorNegativeItem = @colorNegativeItem,
            DataGridFontSize = @dataGridFontSize, ChequeListPageSize = @chequeListPageSize, ChequeListQueryOrderType = @chequeListQueryOrderType, ChequeNotifyDays = @chequeNotifyDays,
            ChequeNotify = @chequeNotify, InvoicePageSize = @invoicePageSize, InvoiceListQueryOrderType = @invoiceListQueryOrderType, InvoiceDetailQueryOrderType = @invoiceDetailQueryOrderType,
            TransactionPageSize = @transactionPageSize, TransactionListQueryOrderType = @transactionListQueryOrderType, AutoSelectPersianLanguage = @autoSelectPersianLanguage,
            TransactionShortcut1Id = @transactionShortcut1Id, TransactionShortcut2Id = @transactionShortcut2Id, TransactionShortcut3Id = transactionShortcut3Id,
            TransactionShortcut1Name = @transactionShortcut1Name, TransactionShortcut2Name = @transactionShortcut2Name, TransactionShortcut3Name = @transactionShortcut3Name,
            AskToAddNotExistingProduct = @askToAddNotExistingProduct, CanViewNetProfits = @canViewNetProfits, CanUseBarcodeReader = @canUseBarcodeReader WHERE Username = @username;";
        private readonly string SelectUserInfo = @"SELECT FirstName, LastName, DateCreated, LastLoginDate FROM UserInfo WHERE Username = @username";
        private readonly string SelectUserPermissions = @"SELECT * FROM UserPermissions WHERE Username = @username";
        private readonly string SelectUserSettings = @"SELECT * FROM UserSettings WHERE Username = @username";
        private readonly string GetPasswordHash = @"SELECT PasswordHash FROM UserInfo WHERE Username = @username";
        private readonly string GetPasswordSalt = @"SELECT PasswordSalt FROM UserInfo WHERE Username = @username";
        private readonly string DeleteUserFromDB = @"DELETE FROM UserInfo WHERE Username = @username; DELETE FROM UserPermissions WHERE Username = @username; DELETE FROM UserSettings WHERE Username = @username";

        public async Task<UserInfo> CreateUser(User_DTO_CreateUpdate user)
        {
            if (user == null || user.Permissions == null || user.Settings == null) return null;
            CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            UserInfo newUser = new()
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Permissions = user.Permissions,
                Settings = user.Settings,
                DateCreated = PersianCalendarModel.GetCurrentPersianDate(),
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt
            };
            var dp = FillParameters(user);
            dp.Add("@passwordHash", newUser.PasswordHash);
            dp.Add("@passwordSalt", newUser.PasswordSalt);
            dp.Add("@dateCreated", newUser.DateCreated);

            var AffectedCount = await DataAccess.SaveDataAsync(CreateUserQuery, dp);
            if (AffectedCount > 0) return newUser; else return null;
        }

        public async Task<bool> VerifyUser(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return false;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var PasswordHash = await DataAccess.QuerySingleOrDefaultAsync<string, DynamicParameters>(GetPasswordHash, dp);
            var PasswordSalt = await DataAccess.QuerySingleOrDefaultAsync<string, DynamicParameters>(GetPasswordSalt, dp);
            if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(PasswordSalt)) return false;
            if (!VerifyPasswordHash(user.Password, Encoding.UTF8.GetBytes(PasswordHash), Encoding.UTF8.GetBytes(PasswordSalt))) return false;
            return true;
        }

        public async Task<LoggedInUser_DTO> GetUserByCredencials(UserLogin_DTO user)
        {
            if (string.IsNullOrEmpty(user.Username)) return null;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            LoggedInUser_DTO loggedUser = await DataAccess.QuerySingleOrDefaultAsync<LoggedInUser_DTO, DynamicParameters>(SelectUserInfo, dp);
            if (loggedUser == null) return null;
            loggedUser.Permissions = await DataAccess.QuerySingleOrDefaultAsync<UserPermissions, DynamicParameters>(SelectUserPermissions, dp);
            if (loggedUser.Permissions == null) return null;
            loggedUser.Settings = await DataAccess.QuerySingleOrDefaultAsync<UserSettings, DynamicParameters>(SelectUserSettings, dp);
            if (loggedUser.Settings == null) return null;
            return loggedUser;
        }

        public async Task<UserInfo> UpdateUser(User_DTO_CreateUpdate user)
        {
            if (user == null || user.Permissions == null || user.Settings == null) return null;
            CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            UserInfo newUser = new()
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

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] oldPasswordHash, byte[] oldPasswordSalt)
        {
            using (var hmac = new HMACSHA512(oldPasswordSalt))
            {
                var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return ComputedHash.SequenceEqual(oldPasswordHash);
            }
        }

        private DynamicParameters FillParameters(User_DTO_CreateUpdate user)
        {
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            dp.Add("@firstName", user.FirstName);
            dp.Add("@lastName", user.LastName);

            dp.Add("@canViewCustomers", user.Permissions.CanViewCustomers);
            dp.Add("@canViewProducts", user.Permissions.CanViewProducts);
            dp.Add("@canViewInvoicesList", user.Permissions.CanViewInvoicesList);
            dp.Add("@canViewInvoiceDetails", user.Permissions.CanViewInvoiceDetails);
            dp.Add("@canViewTransactionsList", user.Permissions.CanViewTransactionsList);
            dp.Add("@canViewTransactionDetails", user.Permissions.CanViewTransactionDetails);
            dp.Add("@canViewCheques", user.Permissions.CanViewCheques);
            dp.Add("@canAddNewCustomer", user.Permissions.CanAddNewCustomer);
            dp.Add("@canAddNewProduct", user.Permissions.CanAddNewProduct);
            dp.Add("@canAddNewInvoice", user.Permissions.CanAddNewInvoice);
            dp.Add("@canAddNewTransaction", user.Permissions.CanAddNewTransaction);
            dp.Add("@canAddNewCheque", user.Permissions.CanAddNewCheque);
            dp.Add("@canEditCustomers", user.Permissions.CanEditCustomers);
            dp.Add("@canEditProducts", user.Permissions.CanEditProducts);
            dp.Add("@canEditInvoices", user.Permissions.CanEditInvoices);
            dp.Add("@canEditTransactions", user.Permissions.CanEditTransactions);
            dp.Add("@canEditCheques", user.Permissions.CanEditCheques);
            dp.Add("@canDeleteCustomer", user.Permissions.CanDeleteCustomer);
            dp.Add("@canDeleteProduct", user.Permissions.CanDeleteProduct);
            dp.Add("@canDeleteInvoice", user.Permissions.CanDeleteInvoice);
            dp.Add("@canDeleteInvoiceItem", user.Permissions.CanDeleteInvoiceItem);
            dp.Add("@canDeleteTransaction", user.Permissions.CanDeleteTransaction);
            dp.Add("@canDeleteTransactionItem", user.Permissions.CanDeleteTransactionItem);
            dp.Add("@canDeleteCheque", user.Permissions.CanDeleteCheque);
            dp.Add("@canPrintInvoice", user.Permissions.CanPrintInvoice);
            dp.Add("@canPrintTransaction", user.Permissions.CanPrintTransaction);
            dp.Add("@canChangeItsSettings", user.Permissions.CanChangeItsSettings);
            dp.Add("@canChangeItsPassword", user.Permissions.CanChangeItsPassword);
            dp.Add("@canAddUser", user.Permissions.CanAddUser);
            dp.Add("@canEditOtherUsersPermission", user.Permissions.CanEditOtherUsersPermission);
            dp.Add("@canEditOtherUsersSettings", user.Permissions.CanEditOtherUsersSettings);

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
            dp.Add("@transactionPageSize", user.Settings.TransactionPageSize);
            dp.Add("@transactionListQueryOrderType", user.Settings.TransactionListQueryOrderType);
            dp.Add("@autoSelectPersianLanguage", user.Settings.AutoSelectPersianLanguage);
            dp.Add("@transactionShortcut1Id", user.Settings.TransactionShortcut1Id);
            dp.Add("@transactionShortcut2Id", user.Settings.TransactionShortcut2Id);
            dp.Add("@transactionShortcut3Id", user.Settings.TransactionShortcut3Id);
            dp.Add("@transactionShortcut1Name", user.Settings.TransactionShortcut1Name);
            dp.Add("@transactionShortcut2Name", user.Settings.TransactionShortcut2Name);
            dp.Add("@transactionShortcut3Name", user.Settings.TransactionShortcut3Name);
            dp.Add("@askToAddNotExistingProduct", user.Settings.AskToAddNotExistingProduct);
            dp.Add("@canViewNetProfits", user.Settings.CanViewNetProfits);
            dp.Add("@canUseBarcodeReader", user.Settings.CanUseBarcodeReader);
            
            return dp;
        }
    }
}