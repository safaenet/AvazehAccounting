using AvazehWeb.Logics;
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
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICustomerCollectionManager manager)
        {
            Manager = manager;
        }

        private readonly ICustomerCollectionManager Manager;

        //GET /Product?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<ProductItemsCollection_DTO>> GetItemsAsync(int Page = 1, string SearchText = "")
        {
            Manager.GenerateWhereClause(SearchText);
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<CustomerModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult> CreateItemAsync(ProductModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            var validate = Manager.Processor.ValidateItem(newItem);
            if (!validate.IsValid) return BadRequest("Model is not valid");
            await Manager.Processor.CreateItemAsync(newItem);
            return Ok("Successfully created the new item");
        }

        [HttpPut("{ID}")]
        public async Task<ActionResult> UpdateItemAsync(int ID, ProductModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            var validate = Manager.Processor.ValidateItem(updatedModel);
            if (!validate.IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = ID;
            if (await Manager.Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
            return Ok("Successfully updated the item");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}