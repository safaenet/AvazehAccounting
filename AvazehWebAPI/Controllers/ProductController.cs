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
        public IProductCollectionManager GetSearch(int Id = 1, string SearchText = "")
        {
            Manager.GenerateWhereClause(SearchText);
            Manager.GotoPage(Id);
            return Manager;
        }

        //[HttpPost]
        //public ActionResult CreateNew()
        //{

        //}
    }
}