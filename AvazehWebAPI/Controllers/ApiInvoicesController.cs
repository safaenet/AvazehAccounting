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
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : ControllerBase
{
    public InvoicesController(IInvoiceProcessor processor)
    {
        Processor = processor;
    }

    private readonly IInvoiceProcessor Processor;

    [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoicesList))]
    public async Task<ActionResult<List<InvoiceListModel>>> GetItemsAsync(int FetcheSize = 50, int InvoiceId = -1 , int CustomerId = -1 , string InvoiceDate = "%", string SearchValue = "%", InvoiceLifeStatus? LifeStatus = InvoiceLifeStatus.Active, InvoiceFinancialStatus? FinStatus = InvoiceFinancialStatus.Outstanding, SqlQuerySearchMode SearchMode = SqlQuerySearchMode.Backward, OrderType orderType = OrderType.DESC, int StartId = -1)
    {
        if (InvoiceId == 0) InvoiceId = -1;
        if (CustomerId == 0) CustomerId = -1;
        if (InvoiceDate == null) InvoiceDate = "%";
        if (SearchValue == null) SearchValue = "%";
        if (StartId == 0) StartId = -1;
        var result = await Processor.LoadManyItemsAsync(FetcheSize, InvoiceId, CustomerId, InvoiceDate, SearchValue, LifeStatus, FinStatus, SearchMode, orderType, StartId);
        return (result == null || !result.Any()) ? NotFound("List is empty") : result.ToList();
    }

    [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoiceDetails))]
    public async Task<ActionResult<InvoiceModel>> GetItemAsync(int Id)
    {
        InvoiceModel model = await Processor.LoadSingleItemAsync(Id);
        return model is null ? NotFound("Couldn't find specific Item") : model;
    }

    [HttpGet("ProductItems"), Authorize]
    public async Task<ActionResult<List<ItemsForComboBox>>> GetProductItemsAsync(string SearchText)
    {
        var items = await Processor.GetProductItemsAsync(SearchText);
        return items is null ? NotFound("Couldn't find any match") : items.ToList();
    }

    [HttpGet("ProductUnits"), Authorize]
    public async Task<ActionResult<List<ProductUnitModel>>> GetProductUnitsAsync(string SearchText)
    {
        var items = await Processor.GetProductUnitsAsync();
        return items is null ? NotFound("List is Empty") : items.ToList();
    }

    [HttpGet("CustomerNames"), Authorize]
    public async Task<ActionResult<List<ItemsForComboBox>>> GetCustomerNamesAsync(string SearchText)
    {
        var items = await Processor.GetCustomerNamesAsync(SearchText);
        return items is null ? NotFound("Couldn't find any match") : items.ToList();
    }

    [HttpGet("CustomerBalance/{CustomerId}/{InvoiceId}"), Authorize]
    public async Task<ActionResult<double>> GetCustomerBalanceAsync(int CustomerId, int InvoiceId = 0)
    {
        var result = await Processor.GetTotalOrRestTotalBalanceOfCustomerAsync(CustomerId, InvoiceId);
        return result;
    }

    [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanAddNewInvoice))]
    public async Task<ActionResult<InvoiceModel>> CreateItemAsync(InvoiceModel_DTO_Create_Update model)
    {
        var newItem = model.AsDaL();
        if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
        await Processor.CreateItemAsync(newItem);
        return newItem;
    }

    [HttpPut("SetPrevInvoiceId/{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<DtoModel<int>>> UpdatePrevInvoiceIdAsync(int Id, DtoModel<int> PrevId)
    {
        var result = await Processor.SetPrevInvoiceId(Id, PrevId.Value);
        if (result == 0) return NotFound();
        return new DtoModel<int>() { Value = result };
    }

    [HttpGet("PrevBalance/{InvoiceId}"), Authorize]
    public async Task<ActionResult<double>> GetPrevBalanceAsync(int InvoiceId)
    {
        var result = await Processor.GetPrevBalanceOfInvoiceAsync(InvoiceId);
        return result;
    }

    [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoiceModel>> UpdateItemAsync(int Id, InvoiceModel_DTO_Create_Update model)
    {
        if (model is null) return BadRequest("Model is not valid");
        var updatedModel = model.AsDaL();
        if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
        updatedModel.Id = Id;
        if (await Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
        return updatedModel;
    }

    [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanDeleteInvoice))]
    public async Task<ActionResult> DeleteItemAsync(int Id)
    {
        if (await Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
        return NotFound("Item not found");
    }
}