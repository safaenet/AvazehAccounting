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
    public class CustomerController<TDal, TDto, TCollection, TManager> : ControllerBase
        where TDal : CustomerModel where TDto : CustomerModel_DTO_Create_Update
        where TCollection : ItemsCollection_DTO<TDal> where TManager : ICollectionManager<TDal, IProcessor<TDal>>
    {
        public CustomerController(TManager manager)
        {
            Manager = manager;
        }

        private readonly TManager Manager;

        //GET /Customer?Id=1&SearchText=sometext
        [HttpGet]
        public async Task<ActionResult<TCollection>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "FirstName", OrderType orderType = OrderType.ASC, int PageSize = 50)
        {
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType);
            Manager.PageSize = PageSize;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto() as TCollection;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<TDal>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<TDal>> CreateItemAsync(TDto model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem as TDal).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem as TDal);
            return newItem as TDal;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<TDal>> UpdateItemAsync(int Id, TDto model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel as TDal).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel as TDal) == 0) return NotFound();
            return updatedModel as TDal;
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}