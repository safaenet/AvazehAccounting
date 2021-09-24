using AvazehWeb;
using AvazehWeb.Models;
using DataLibraryCore.DataAccess.CollectionManagers;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        public ProductController(IProductCollectionManager manager)
        {
            Manager = manager;
        }

        private readonly IProductCollectionManager Manager;

        //GET /Product?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<ProductItemsCollection_DTO>> GetItemsAsync(int Page = 1, string SearchText = "", int PageSize = 50)
        {
            Manager.GenerateWhereClause(SearchText);
            Manager.PageSize = PageSize;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ProductModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ProductModel>> CreateItemAsync(ProductModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if(!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<ProductModel>> UpdateItemAsync(int Id, ProductModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}