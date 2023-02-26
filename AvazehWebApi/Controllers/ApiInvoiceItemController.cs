using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoiceItemController : ControllerBase
{
    public InvoiceItemController(IInvoiceProcessor processor)
    {
        Processor = processor;
    }

    private readonly IInvoiceProcessor Processor;

    [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoiceDetails))]
    public async Task<ActionResult<InvoiceItemModel>> GetItemAsync(int Id)
    {
        var item = await Processor.GetInvoiceItemFromDatabaseAsync(Id);
        if (item is null) return NotFound("Couldn't find specific Item");
        return item;
    }

    [HttpGet("{MaxRecord}/{CustomerId}/{ProductId}"), Authorize]
    public async Task<ActionResult<ObservableCollection<RecentSellPriceModel>>> GetItemAsync(int MaxRecord, int CustomerId, int ProductId) //Used for getting recent sell prices.
    {
        var items = await Processor.GetRecentSellPricesAsync(MaxRecord, CustomerId, ProductId);
        if (items is null || items.Count == 0) return NotFound("Couldn't find specific Item");
        return items;
    }

    [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoiceItemModel>> CreateItemAsync(InvoiceItemModel_DTO_Create_Update model)
    {
        var newItem = model.AsDaL();
        if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
        await Processor.InsertInvoiceItemToDatabaseAsync(newItem);
        return newItem;
    }

    [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoiceItemModel>> UpdateItemAsync(int Id, InvoiceItemModel_DTO_Create_Update model)
    {
        if (model is null) return BadRequest("Model is not valid");
        var updatedModel = model.AsDaL();
        if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
        updatedModel.Id = Id;
        if (await Processor.UpdateInvoiceItemInDatabaseAsync(updatedModel) == 0) return NotFound();
        return updatedModel;
    }

    [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult> DeleteItemAsync(int Id)
    {
        if (await Processor.DeleteInvoiceItemFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
        return NotFound("Item not found");
    }
}