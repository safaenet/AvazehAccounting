using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : ControllerBase
{
    public InvoicesController(IInvoiceCollectionManager manager)
    {
        Manager = manager;
    }

    private readonly IInvoiceCollectionManager Manager;

    //GET /Customer?Id=1&SearchText=sometext
    [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoicesList))]
    public async Task<ActionResult<ItemsCollection_DTO<InvoiceListModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "Id", OrderType orderType = OrderType.DESC, InvoiceLifeStatus? LifeStatus = InvoiceLifeStatus.Active, InvoiceFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false)
    {
        Manager.GenerateWhereClause(SearchText, OrderBy, orderType, LifeStatus, FinStatus);
        Manager.PageSize = PageSize;
        if (ForceLoad) Manager.Initialized = false;
        await Manager.GotoPageAsync(Page);
        if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
        return Manager.AsDto();
    }

    [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoiceDetails))]
    public async Task<ActionResult<InvoiceModel>> GetItemAsync(int Id)
    {
        InvoiceModel model = await Manager.Processor.LoadSingleItemAsync(Id);
        return model is null ? NotFound("Couldn't find specific Item") : model;
    }

    [HttpGet("ProductItems"), Authorize]
    public async Task<ActionResult<List<ItemsForComboBox>>> GetProductItemsAsync(string SearchText)
    {
        var items = await Manager.Processor.GetProductItemsAsync(SearchText);
        return items is null ? NotFound("Couldn't find any match") : items;
    }

    [HttpGet("ProductUnits"), Authorize]
    public async Task<ActionResult<List<ProductUnitModel>>> GetProductUnitsAsync(string SearchText)
    {
        var items = await Manager.Processor.GetProductUnitsAsync();
        return items is null ? NotFound("List is Empty") : items;
    }

    [HttpGet("CustomerNames"), Authorize]
    public async Task<ActionResult<List<ItemsForComboBox>>> GetCustomerNamesAsync(string SearchText)
    {
        var items = await Manager.Processor.GetCustomerNamesAsync(SearchText);
        return items is null ? NotFound("Couldn't find any match") : items;
    }

    [HttpGet("CustomerBalance/{CustomerId}/{InvoiceId}"), Authorize]
    public async Task<ActionResult<double>> GetCustomerBalanceAsync(int CustomerId, int InvoiceId = 0)
    {
        var result = await Manager.Processor.GetTotalOrRestTotalBalanceOfCustomerAsync(CustomerId, InvoiceId);
        return result;
    }

    [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanAddNewInvoice))]
    public async Task<ActionResult<InvoiceModel>> CreateItemAsync(InvoiceModel_DTO_Create_Update model)
    {
        var newItem = model.AsDaL();
        if (!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
        await Manager.Processor.CreateItemAsync(newItem);
        return newItem;
    }

    [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoiceModel>> UpdateItemAsync(int Id, InvoiceModel_DTO_Create_Update model)
    {
        if (model is null) return BadRequest("Model is not valid");
        var updatedModel = model.AsDaL();
        if (!Manager.Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
        updatedModel.Id = Id;
        if (await Manager.Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
        return updatedModel;
    }

    [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanDeleteInvoice))]
    public async Task<ActionResult> DeleteItemAsync(int Id)
    {
        if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
        return NotFound("Item not found");
    }
}