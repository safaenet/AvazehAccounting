using AvazehWebAPI.Models;
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
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        public ProductController(IProductCollectionManager manager)
        {
            Manager = manager;
        }

        private readonly IProductCollectionManager Manager;

        //GET /Product?Id=1&SearchText=sometext
        [HttpGet]
        public ActionResult<ProductItemsCollection_DTO> GetItems(int Page = 1, string SearchText = "")
        {
            Manager.GenerateWhereClause(SearchText);
            Manager.GotoPage(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}")]
        public ActionResult<ProductModel> GetItem(int Id)
        {
            var item = Manager.Processor.LoadSingleItem(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public ActionResult CreateItem(ProductModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaLModel();
            Manager.Processor.CreateItem(newItem);
            return Ok("Successfully created the new item");
        }

        [HttpPut("{ID}")]
        public ActionResult UpdateItem(int ID, ProductModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaLModel();
            updatedModel.Id = ID;
            if (Manager.Processor.UpdateItem(updatedModel) == 0) return NotFound();
            return Ok("Successfully updated the item");
        }

        [HttpDelete]
        public ActionResult DeleteItem(int Id)
        {
            if (Manager.Processor.DeleteItemById(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}