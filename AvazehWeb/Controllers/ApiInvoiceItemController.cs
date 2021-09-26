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
    [Route("api/v1/[controller]")]
    public class InvoiceItemController : ControllerBase
    {
        public InvoiceItemController(ICollectionManager<InvoiceItemModel, IProcessor<InvoiceItemModel>> manager)
        {
            Manager = manager;
        }

        private readonly ICollectionManager<InvoiceItemModel, IProcessor<InvoiceItemModel>> Manager;

        [HttpGet("{Id}")]
        public async Task<ActionResult<InvoiceItemModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItemModel>> CreateItemAsync(InvoiceItemModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem as InvoiceItemModel).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem as InvoiceItemModel);
            return newItem as InvoiceItemModel;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<InvoiceItemModel>> UpdateItemAsync(int Id, InvoiceItemModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel as InvoiceItemModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel as InvoiceItemModel) == 0) return NotFound();
            return updatedModel as InvoiceItemModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}