using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;

namespace AvazehWeb.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionController : ControllerBase
    {
        public TransactionController(ICollectionManager<TransactionModel, IProcessor<TransactionModel>> manager)
        {
            Manager = manager;
        }

        private readonly ICollectionManager<TransactionModel, IProcessor<TransactionModel>> Manager;

        //GET /Product?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<ItemsCollection_DTO<TransactionModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "Id", OrderType orderType = OrderType.ASC, int PageSize = 50, bool ForceLoad = false)
        {
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType);
            Manager.PageSize = PageSize;
            if (ForceLoad) Manager.Initialized = false;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<TransactionModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionModel>> CreateItemAsync(TransactionModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<TransactionModel>> UpdateItemAsync(int Id, TransactionModel_DTO_Create_Update model)
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