using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionItemController : ControllerBase
    {
        public TransactionItemController(ITransactionProcessor processor)
        {
            Processor = processor;
        }

        private readonly ITransactionProcessor Processor;

        [HttpGet("{Id}")]
        public async Task<ActionResult<TransactionItemModel>> GetItemAsync(int Id)
        {
            var item = await Processor.GetTransactionItemFromDatabaseAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionItemModel>> CreateItemAsync(TransactionItemModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Processor.InsertTransactionItemToDatabaseAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<TransactionItemModel>> UpdateItemAsync(int Id, TransactionItemModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Processor.UpdateTransactionItemInDatabaseAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Processor.DeleteTransactionItemFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}