using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicePaymentController : ControllerBase
{
    public InvoicePaymentController(IInvoiceProcessor processor)
    {
        Processor = processor;
    }

    private readonly IInvoiceProcessor Processor;

    [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewInvoiceDetails))]
    public async Task<ActionResult<InvoicePaymentModel>> GetItemAsync(int Id)
    {
        var item = await Processor.GetInvoicePaymentFromDatabaseAsync(Id);
        if (item is null) return NotFound("Couldn't find specific Item");
        return item;
    }

    [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoicePaymentModel>> CreateItemAsync(InvoicePaymentModel_DTO_Create_Update model)
    {
        var newItem = model.AsDaL();
        if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
        await Processor.InsertInvoicePaymentToDatabaseAsync(newItem);
        return newItem;
    }

    [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult<InvoicePaymentModel>> UpdateItemAsync(int Id, InvoicePaymentModel_DTO_Create_Update model)
    {
        if (model is null) return BadRequest("Model is not valid");
        var updatedModel = model.AsDaL();
        if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
        updatedModel.Id = Id;
        if (await Processor.UpdateInvoicePaymentInDatabaseAsync(updatedModel) == 0) return NotFound();
        return updatedModel;
    }

    [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditInvoice))]
    public async Task<ActionResult> DeleteItemAsync(int Id)
    {
        if (await Processor.DeleteInvoicePaymentFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
        return NotFound("Item not found");
    }
}