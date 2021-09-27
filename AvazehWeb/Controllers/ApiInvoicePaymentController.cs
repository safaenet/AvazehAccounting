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
    public class InvoicePaymentController : ControllerBase
    {
        public InvoicePaymentController(IInvoiceProcessor processor)
        {
            Processor = processor;
        }

        private readonly IInvoiceProcessor Processor;

        [HttpGet("{Id}")]
        public async Task<ActionResult<InvoicePaymentModel>> GetItemAsync(int Id)
        {
            var item = await Processor.GetInvoicePaymentFromDatabaseAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<InvoicePaymentModel>> CreateItemAsync(InvoicePaymentModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Processor.InsertInvoicePaymentToDatabaseAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<InvoicePaymentModel>> UpdateItemAsync(int Id, InvoicePaymentModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Processor.UpdateInvoicePaymentInDatabaseAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Processor.DeleteInvoicePaymentFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}