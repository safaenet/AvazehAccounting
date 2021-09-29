using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InvoiceItemController : ControllerBase
    {
        public InvoiceItemController(IInvoiceProcessor processor)
        {
            Processor = processor;
        }

        private readonly IInvoiceProcessor Processor;

        [HttpGet("{Id}")]
        public async Task<ActionResult<InvoiceItemModel>> GetItemAsync(int Id)
        {
            var item = await Processor.GetInvoiceItemFromDatabaseAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItemModel>> CreateItemAsync(InvoiceItemModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Processor.InsertInvoiceItemToDatabaseAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<InvoiceItemModel>> UpdateItemAsync(int Id, InvoiceItemModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Processor.UpdateInvoiceItemInDatabaseAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Processor.DeleteInvoiceItemFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}