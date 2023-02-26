using Dapper;
using System.Linq;
using DataLibraryCore.DataAccess.Interfaces;
using System.Threading.Tasks;
using SharedLibrary.SecurityAndSettingsModels;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.ObjectModel;
using SharedLibrary.Helpers;
using Serilog;

namespace DataLibraryCore.DataAccess.SqlServer;

public class SqlUserProcessor : IUserProcessor
{
    public SqlUserProcessor(IDataAccess dataAccess)
    {
        DataAccess = dataAccess;
    }

    private readonly IDataAccess DataAccess;
    private readonly string CreateUserQuery = @"DECLARE @newId int; SET @newId = (SELECT ISNULL(MAX([Id]), 0) FROM [UserInfo]) + 1;
            INSERT INTO UserInfo ([Id], [Username], PasswordHash, PasswordSalt, FirstName, LastName, DateCreated, IsActive)
            VALUES (@newId, @username, @passwordHash, @passwordSalt, @firstName, @lastName, @dateCreated, @isActive);

            INSERT INTO UserPermissions ([Id], CanViewCustomersList, CanViewCustomerDetails, CanViewProductsList, CanViewProductDetails, CanViewInvoicesList, CanViewInvoiceDetails, CanViewTransactionsList, CanViewTransactionDetails,
            CanViewChequesList, CanViewChequeDetails, CanAddNewCustomer, CanAddNewProduct, CanAddNewInvoice, CanAddNewTransaction, CanAddNewCheque, CanEditCustomer, CanEditProduct, CanEditInvoice,
            CanEditTransaction, CanEditCheque, CanDeleteCustomer, CanDeleteProduct, CanDeleteInvoice, CanDeleteInvoiceItem, CanDeleteTransaction, CanDeleteTransactionItem,
            CanDeleteCheque, CanPrintInvoice, CanPrintTransaction, CanViewNetProfits, CanUseBarcodeReader, CanManageItself, CanManageOthers)
            
            VALUES (@newId, @canViewCustomersList, @canViewCustomerDetails, @canViewProductsList, @canViewProductDetails, @canViewInvoicesList, @canViewInvoiceDetails, @canViewTransactionsList, @canViewTransactionDetails,
            @canViewChequesList, @canViewChequeDetails, @canAddNewCustomer, @canAddNewProduct, @canAddNewInvoice, @canAddNewTransaction, @canAddNewCheque, @canEditCustomer, @canEditProduct, @canEditInvoice,
            @canEditTransaction, @canEditCheque, @canDeleteCustomer, @canDeleteProduct, @canDeleteInvoice, @canDeleteInvoiceItem, @canDeleteTransaction, @canDeleteTransactionItem,
            @canDeleteCheque, @canPrintInvoice, @canPrintTransaction, @canViewNetProfits, @canUseBarcodeReader, @canManageItself, @canManageOthers);

            INSERT INTO UserSettings ([Id], ColorNewItem, ColorSoldItem, ColorNonSufficientFundItem, ColorCashedItem, ColorChequeNotification, ColorUpdatedItem,
            ColorBalancedItem, ColorDeptorItem, ColorCreditorItem, ColorInactiveItem, ColorArchivedItem, ColorDeletedItem, ColorNegativeProfit, ColorPositiveItem, ColorNegativeItem,
            DataGridFontSize, ChequeListPageSize, ChequeListQueryOrderType, ChequeNotifyDays, ChequeNotify, InvoicePageSize, InvoiceListQueryOrderType, InvoiceDetailQueryOrderType,
            TransactionListPageSize, TransactionDetailPageSize, TransactionListQueryOrderType, TransactionDetailQueryOrderType, AutoSelectPersianLanguage, TransactionShortcut1Id, TransactionShortcut2Id, TransactionShortcut3Id,
            TransactionShortcut1Name, TransactionShortcut2Name, TransactionShortcut3Name, AskToAddNotExistingProduct, SearchWhenTyping, CustomerListPageSize, CustomerListQueryOrderType, ProductListPageSize, ProductListQueryOrderType)
            
            VALUES (@newId, @colorNewItem, @colorSoldItem, @colorNonSufficientFundItem, @colorCashedItem, @colorChequeNotification, @colorUpdatedItem,
            @colorBalancedItem, @colorDeptorItem, @colorCreditorItem, @colorInactiveItem, @colorArchivedItem, @colorDeletedItem, @colorNegativeProfit, @colorPositiveItem, @colorNegativeItem,
            @dataGridFontSize, @chequeListPageSize, @chequeListQueryOrderType, @chequeNotifyDays, @chequeNotify, @invoicePageSize, @invoiceListQueryOrderType, @invoiceDetailQueryOrderType,
            @transactionListPageSize, @transactionDetailPageSize, @transactionListQueryOrderType, @transactionDetailQueryOrderType, @autoSelectPersianLanguage, @transactionShortcut1Id, @transactionShortcut2Id, @transactionShortcut3Id,
            @transactionShortcut1Name, @transactionShortcut2Name, @transactionShortcut3Name, @askToAddNotExistingProduct, @searchWhenTyping, @customerListPageSize, @customerListQueryOrderType, @productListPageSize, @productListQueryOrderType);
            SELECT @id = @newId;";

    private static readonly string UpdateUserInfoQueryWithPassword = @"UPDATE UserInfo SET Username = @username, PasswordHash = @passwordHash, PasswordSalt = @passwordSalt, FirstName = @firstName, LastName = @lastName, IsActive = @isActive WHERE [Id] = @id;";
    private static readonly string UpdateUserInfoQueryWithoutPassword = @"UPDATE UserInfo SET Username = @username, FirstName = @firstName, LastName = @lastName, IsActive = @isActive WHERE [Id] = @id;";
    private static readonly string UpdateUserPermissionsQuery = @"UPDATE UserPermissions SET CanViewCustomersList = @canViewCustomersList, CanViewCustomerDetails = @canViewCustomerDetails, CanViewProductsList = @canViewProductsList, CanViewProductDetails = @canViewProductDetails,
            CanViewInvoicesList = @canViewInvoicesList, CanViewInvoiceDetails = @canViewInvoiceDetails, CanViewTransactionsList = @canViewTransactionsList, CanViewTransactionDetails = @canViewTransactionDetails,
            CanViewChequesList = @canViewChequesList, CanViewChequeDetails = @canViewChequeDetails, CanAddNewCustomer = @canAddNewCustomer, CanAddNewProduct = @canAddNewProduct, CanAddNewInvoice = @canAddNewInvoice,
            CanAddNewTransaction = @canAddNewTransaction, CanAddNewCheque = @canAddNewCheque, CanEditCustomer = @canEditCustomer, CanEditProduct = @canEditProduct,
            CanEditInvoice = @canEditInvoice, CanEditTransaction = @canEditTransaction, CanEditCheque = @canEditCheque, CanDeleteCustomer = @canDeleteCustomer,
            CanDeleteProduct = @canDeleteProduct, CanDeleteInvoice = @canDeleteInvoice, CanDeleteInvoiceItem = @canDeleteInvoiceItem, CanDeleteTransaction = @canDeleteTransaction,
            CanDeleteTransactionItem = @canDeleteTransactionItem, CanDeleteCheque = @canDeleteCheque, CanPrintInvoice = @canPrintInvoice, CanPrintTransaction = @canPrintTransaction,
            CanViewNetProfits = @canViewNetProfits, CanUseBarcodeReader = @canUseBarcodeReader, CanManageItself = @canManageItself, CanManageOthers = @canManageOthers WHERE [Id] = @id;";
    private static readonly string UpdateUserSettingsQuery = @"UPDATE UserSettings SET ColorNewItem = @colorNewItem, ColorSoldItem = @colorSoldItem, ColorNonSufficientFundItem = @colorNonSufficientFundItem,
            ColorCashedItem = @colorCashedItem, ColorChequeNotification = @colorChequeNotification, ColorUpdatedItem = @colorUpdatedItem, ColorBalancedItem = @colorBalancedItem,
            ColorDeptorItem = @colorDeptorItem, ColorCreditorItem = @colorCreditorItem, ColorInactiveItem = @colorInactiveItem, ColorArchivedItem = @colorArchivedItem,
            ColorDeletedItem = @colorDeletedItem, ColorNegativeProfit = @colorNegativeProfit, ColorPositiveItem = @colorPositiveItem, ColorNegativeItem = @colorNegativeItem,
            DataGridFontSize = @dataGridFontSize, ChequeListPageSize = @chequeListPageSize, ChequeListQueryOrderType = @chequeListQueryOrderType, ChequeNotifyDays = @chequeNotifyDays,
            ChequeNotify = @chequeNotify, InvoicePageSize = @invoicePageSize, InvoiceListQueryOrderType = @invoiceListQueryOrderType, InvoiceDetailQueryOrderType = @invoiceDetailQueryOrderType,
            TransactionListPageSize = @transactionListPageSize, TransactionDetailPageSize = @transactionDetailPageSize, TransactionListQueryOrderType = @transactionListQueryOrderType,
            TransactionDetailQueryOrderType = @transactionDetailQueryOrderType, AutoSelectPersianLanguage = @autoSelectPersianLanguage,
            TransactionShortcut1Id = @transactionShortcut1Id, TransactionShortcut2Id = @transactionShortcut2Id, TransactionShortcut3Id = transactionShortcut3Id,
            TransactionShortcut1Name = @transactionShortcut1Name, TransactionShortcut2Name = @transactionShortcut2Name, TransactionShortcut3Name = @transactionShortcut3Name,
            AskToAddNotExistingProduct = @askToAddNotExistingProduct, SearchWhenTyping = @searchWhenTyping, CustomerListPageSize = @customerListPageSize, CustomerListQueryOrderType = @customerListQueryOrderType,
            ProductListPageSize = @productListPageSize, ProductListQueryOrderType = @productListQueryOrderType WHERE [Id] = @id;";
    private readonly string SelectUserInfoBase = @"SELECT [Id], [Username], FirstName, LastName, DateCreated, LastLoginDate, LastLoginTime, IsActive FROM UserInfo WHERE Username = @username";
    private readonly string SelectUserPermissions = @"SELECT * FROM UserPermissions WHERE [Id] = @id";
    private readonly string SelectUserSettings = @"SELECT * FROM UserSettings WHERE [Id] = @id";
    private readonly string GetPasswordHash = @"SELECT PasswordHash FROM UserInfo WHERE Username = @username";
    private readonly string GetPasswordSalt = @"SELECT PasswordSalt FROM UserInfo WHERE Username = @username";
    private readonly string DeleteUserFromDB = @"DELETE FROM UserInfo WHERE [Id] = @id; DELETE FROM UserPermissions WHERE [Id] = @id; DELETE FROM UserSettings WHERE [Id] = @id";
    private readonly string GetUsersListQuery = @"SELECT [Id], Username, FirstName, LastName, DateCreated, LastLoginDate, LastLoginTime, IsActive FROM UserInfo";
    private readonly string GetCountOfAdminUsersQuery = @"SELECT COUNT(u.Username) FROM UserInfo u LEFT JOIN UserPermissions p ON u.Id = p.Id WHERE p.CanManageOthers = 1 AND u.IsActive = 1";
    private readonly string UpdateUserLastLoginDateQuery = @"UPDATE UserInfo SET LastLoginDate = @lastLoginDate, LastLoginTime = @lastLoginTime WHERE Username = @username";

    public async Task<bool> TestDBConnectionAsync()
    {
        try
        {
            var result = await DataAccess.TestConnectionAsync();
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return false;
    }

    public async Task<ObservableCollection<UserInfoBaseModel>> GetUsersAsync()
    {
        try
        {
            var list = await DataAccess.LoadDataAsync<UserInfoBaseModel, DynamicParameters>(GetUsersListQuery, null);
            return list;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<int> GetCountOfAdminUsersAsync()
    {
        try
        {
            var count = await DataAccess.ExecuteScalarAsync<int, DynamicParameters>(GetCountOfAdminUsersQuery, null);
            return count;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return 0;
    }

    public async Task<UserInfoBaseModel> CreateUserAsync(User_DTO_CreateUpdate user)
    {
        try
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || user.Password.Length < 4 || user.Permissions == null || user.Settings == null) return null;
            CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
            UserInfoBaseModel newUser = new()
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                DateCreated = DateTime.Now
            };
            var dp = new DynamicParameters();
            FillUserBaseParameters(dp, newUser);
            FillUserPermissionParameters(dp, user.Permissions);
            FillUserSettingsParameters(dp, user.Settings);
            dp.Add("@passwordHash", PasswordHash, System.Data.DbType.Binary);
            dp.Add("@passwordSalt", PasswordSalt, System.Data.DbType.Binary);
            dp.Add("@dateCreated", newUser.DateCreated);
            dp.Add("@id", 0, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);

            var AffectedCount = await DataAccess.SaveDataAsync(CreateUserQuery, dp);
            newUser.Id = dp.Get<int>("@id");
            if (AffectedCount > 0) return newUser; else return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<bool> VerifyUserAsync(UserLogin_DTO user)
    {
        try
        {
            if (string.IsNullOrEmpty(user.Username)) return false;
            DynamicParameters dp = new();
            dp.Add("@username", user.Username);
            var PasswordHash = await DataAccess.QuerySingleOrDefaultAsync<byte[], DynamicParameters>(GetPasswordHash, dp);
            var PasswordSalt = await DataAccess.QuerySingleOrDefaultAsync<byte[], DynamicParameters>(GetPasswordSalt, dp);
            if (PasswordHash == null || PasswordSalt == null) return false;
            return VerifyPasswordHash(user.Password, PasswordHash, PasswordSalt);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return false;
    }

    private static bool VerifyPasswordHash(string password, byte[] oldPasswordHash, byte[] oldPasswordSalt)
    {
        try
        {
            using var hmac = new HMACSHA512(oldPasswordSalt);
            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return ComputedHash.SequenceEqual(oldPasswordHash);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return false;
    }

    public async Task<UserInfoBaseModel> GetUserInfoBaseAsync(string Username)
    {
        try
        {
            if (string.IsNullOrEmpty(Username)) return null;
            DynamicParameters dp = new();
            dp.Add("@username", Username);
            var userInfoBase = await DataAccess.QuerySingleOrDefaultAsync<UserInfoBaseModel, DynamicParameters>(SelectUserInfoBase, dp);
            return userInfoBase;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<UserPermissionsModel> GetUserPermissionsAsync(int Id)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            var Permissions = await DataAccess.QuerySingleOrDefaultAsync<UserPermissionsModel, DynamicParameters>(SelectUserPermissions, dp);
            return Permissions;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<UserSettingsModel> GetUserSettingsAsync(int Id)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            var Settings = await DataAccess.QuerySingleOrDefaultAsync<UserSettingsModel, DynamicParameters>(SelectUserSettings, dp);
            return Settings;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<UserInfoBaseModel> UpdateUserInfoAsync(UserInfoBaseModel user, bool ChangePassword = false, string NewPassword = null)
    {
        try
        {
            if (user == null) return null;
            var dp = new DynamicParameters();
            if (ChangePassword)
            {
                CreatePasswordHash(NewPassword, out byte[] PasswordHash, out byte[] PasswordSalt);
                dp.Add("@passwordHash", PasswordHash);
                dp.Add("@passwordSalt", PasswordSalt);
            }
            FillUserBaseParameters(dp, user);
            dp.Add("@id", user.Id);
            var AffectedCount = await DataAccess.SaveDataAsync(ChangePassword ? UpdateUserInfoQueryWithPassword : UpdateUserInfoQueryWithoutPassword, dp);
            if (AffectedCount > 0) return user; else return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<UserPermissionsModel> UpdateUserPermissionsAsync(int Id, UserPermissionsModel userPermissions)
    {
        try
        {
            if (userPermissions == null) return null;
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            FillUserPermissionParameters(dp, userPermissions);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateUserPermissionsQuery, dp);
            if (AffectedCount > 0) return userPermissions; else return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<UserSettingsModel> UpdateUserSettingsAsync(int Id, UserSettingsModel userSettings)
    {
        try
        {
            if (userSettings == null) return null;
            var dp = new DynamicParameters();
            dp.Add("@id", Id);
            FillUserSettingsParameters(dp, userSettings);
            var AffectedCount = await DataAccess.SaveDataAsync(UpdateUserSettingsQuery, dp);
            if (AffectedCount > 0) return userSettings; else return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return null;
    }

    public async Task<int> DeleteUserAsync(int Id)
    {
        try
        {
            DynamicParameters dp = new();
            dp.Add("@id", Id);
            return await DataAccess.SaveDataAsync(DeleteUserFromDB, dp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
        return 0;
    }

    public async Task UpdateUserLastLoginDateAsync(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username)) return;
            DynamicParameters dp = new();
            dp.Add("@username", username);
            dp.Add("@lastLoginDate", PersianCalendarHelper.GetCurrentPersianDate());
            dp.Add("@lastLoginTime", PersianCalendarHelper.GetCurrentTime());
            await DataAccess.SaveDataAsync(UpdateUserLastLoginDateQuery, dp);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SqlUserProcessor");
        }
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static void FillUserBaseParameters(DynamicParameters dp, UserInfoBaseModel user)
    {
        dp.Add("@username", user.Username);
        dp.Add("@firstName", user.FirstName);
        dp.Add("@lastName", user.LastName);
        dp.Add("@isActive", user.IsActive);
    }

    private static void FillUserPermissionParameters(DynamicParameters dp, UserPermissionsModel Permissions)
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

    private static void FillUserSettingsParameters(DynamicParameters dp, UserSettingsModel Settings)
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
        dp.Add("@transactionShortcut1Id", Settings.TransactionShortcut1Id);
        dp.Add("@transactionShortcut2Id", Settings.TransactionShortcut2Id);
        dp.Add("@transactionShortcut3Id", Settings.TransactionShortcut3Id);
        dp.Add("@transactionShortcut1Name", Settings.TransactionShortcut1Name);
        dp.Add("@transactionShortcut2Name", Settings.TransactionShortcut2Name);
        dp.Add("@transactionShortcut3Name", Settings.TransactionShortcut3Name);
        dp.Add("@askToAddNotExistingProduct", Settings.AskToAddNotExistingProduct);
        dp.Add("@searchWhenTyping", Settings.SearchWhenTyping);
        dp.Add("@customerListPageSize", Settings.CustomerListPageSize);
        dp.Add("@customerListQueryOrderType", Settings.CustomerListQueryOrderType);
        dp.Add("@productListPageSize", Settings.ProductListPageSize);
        dp.Add("@productListQueryOrderType", Settings.ProductListQueryOrderType);
    }
}