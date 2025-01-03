﻿using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    public ProductController(IGeneralCollectionManager<ProductModel, IGeneralProcessor<ProductModel>> manager)
    {
        Manager = manager;
    }

    private readonly IGeneralCollectionManager<ProductModel, IGeneralProcessor<ProductModel>> Manager;

    //GET /Product?Id=1&SearchText=sometext
    [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewProductsList))]
    public async Task<ActionResult<ItemsCollection_DTO<ProductModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "ProductName", OrderType orderType = OrderType.ASC, int PageSize = 50, bool ForceLoad = false)
    {
        Manager.GenerateWhereClause(SearchText, OrderBy, orderType);
        Manager.PageSize = PageSize;
        if (ForceLoad) Manager.Initialized = false;
        await Manager.GotoPageAsync(Page);
        if (Manager.Items == null || Manager.Items.Count() == 0) return NotFound("List is empty");
        return Manager.AsDto();
    }

    [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewProductDetails))]
    public async Task<ActionResult<ProductModel>> GetItemAsync(int Id)
    {
        var item = await Manager.Processor.LoadSingleItemAsync(Id);
        if (item is null) return NotFound("Couldn't find specific Item");
        return item;
    }

    [HttpGet("BarCode/{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewProductDetails))]
    public async Task<ActionResult<ProductModel>> GetItemAsync(string Id)
    {
        var item = await Manager.Processor.LoadSingleItemByBarcodeAsync(Id);
        if (item is null) return NotFound("Couldn't find specific Item");
        return item;
    }

    [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanAddNewProduct))]
    public async Task<ActionResult<ProductModel>> CreateItemAsync(ProductModel_DTO_Create_Update model)
    {
        var newItem = model.AsDaL();
        if (!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
        await Manager.Processor.CreateItemAsync(newItem);
        return newItem;
    }

    [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditProduct))]
    public async Task<ActionResult<ProductModel>> UpdateItemAsync(int Id, ProductModel_DTO_Create_Update model)
    {
        if (model is null) return BadRequest("Model is not valid");
        var updatedModel = model.AsDaL();
        if (!Manager.Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
        updatedModel.Id = Id;
        if (await Manager.Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
        return updatedModel;
    }

    [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanDeleteProduct))]
    public async Task<ActionResult> DeleteItemAsync(int Id)
    {
        if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
        return NotFound("Item not found");
    }
}