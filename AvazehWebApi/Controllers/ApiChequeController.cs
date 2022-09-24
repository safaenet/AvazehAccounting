using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChequeController : ControllerBase
    {
        public ChequeController(IChequeCollectionManager manager)
        {
            Manager = manager;
        }

        private readonly IChequeCollectionManager Manager;

        //GET /Cheque?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<ItemsCollection_DTO<ChequeModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "DueDate", OrderType orderType = OrderType.DESC, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, int PageSize = 50, bool ForceLoad = false)
        {
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType, listQueryStatus);
            Manager.PageSize = PageSize;
            if (ForceLoad) Manager.Initialized = false;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ChequeModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet("Banknames")]
        public async Task<ActionResult<List<string>>> GetBanknamesAsync()
        {
            var items = await Manager.Processor.GetBanknames();
            return items is null ? NotFound("Couldn't find any match") : items;
        }

        [HttpPost]
        public async Task<ActionResult<ChequeModel>> CreateItemAsync(ChequeModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem as ChequeModel).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem as ChequeModel);
            return newItem as ChequeModel;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<ChequeModel>> UpdateItemAsync(int Id, ChequeModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel as ChequeModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel as ChequeModel) == 0) return NotFound();
            return updatedModel as ChequeModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}