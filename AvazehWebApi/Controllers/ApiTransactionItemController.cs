using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionItemController : ControllerBase
    {
        public TransactionItemController(ITransactionItemCollectionManager manager)
        {
            Manager = manager;
        }

        private readonly ITransactionItemCollectionManager Manager;

        [HttpGet("{TransactionId}")]
        public async Task<ActionResult<ItemsCollection_DTO<TransactionItemModel>>> GetItemsAsync(int TransactionId, int Page = 1, string SearchText = "", string OrderBy = "Id", OrderType orderType = OrderType.DESC, TransactionFinancialStatus? FinStatus = null, int PageSize = 100, bool ForceLoad = false)
        {
            Manager.TransactionId = TransactionId;
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType, FinStatus);
            Manager.PageSize = PageSize;
            if (ForceLoad) Manager.Initialized = false;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionItemModel>> CreateItemAsync(TransactionItemModel_DTO_Create_Update model)
        {
            //Manager.TransactionId = Id;
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Manager.Processor.InsertTransactionItemToDatabaseAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<TransactionItemModel>> UpdateItemAsync(int Id, TransactionItemModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            Manager.TransactionId = Id;
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateTransactionItemInDatabaseAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            Manager.TransactionId = Id;
            if (await Manager.Processor.DeleteTransactionItemFromDatabaseAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}