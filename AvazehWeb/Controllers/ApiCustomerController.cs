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
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICollectionManager<CustomerModel, IProcessor<CustomerModel>> manager)
        {
            Manager = manager;
        }

        private readonly ICollectionManager<CustomerModel, IProcessor<CustomerModel>> Manager;

        //GET /Customer?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<ItemsCollection_DTO<CustomerModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "FirstName", OrderType orderType = OrderType.ASC, int PageSize = 50)
        {
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType);
            Manager.PageSize = PageSize;
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
        public async Task<ActionResult<CustomerModel>> CreateItemAsync(CustomerModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem as CustomerModel).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem as CustomerModel);
            return newItem as CustomerModel;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<CustomerModel>> UpdateItemAsync(int Id, CustomerModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel as CustomerModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel as CustomerModel) == 0) return NotFound();
            return updatedModel as CustomerModel;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}