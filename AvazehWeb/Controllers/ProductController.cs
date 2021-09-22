using AvazehWeb.Models;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Controllers
{
    public class ProductController : Controller
    {
        public ProductController(IProductCollectionManager manager)
        {
            Manager = manager;
        }
        private readonly IProductCollectionManager Manager;

        // GET: ProductController
        public async Task<ActionResult> Index(int Id = 1, string SearchText = "")
        {
            if (!Manager.Initialized || Manager.SearchValue != SearchText) Manager.GenerateWhereClause(SearchText);
            if (Manager.CurrentPage != Id) await Manager.GotoPageAsync(Id);
            ViewData["CurrentPage"] = Manager.CurrentPage;
            ViewData["Search"] = SearchText;
            ViewData["PagesCount"] = Manager.PagesCount;
            var modelList = Manager.Items;
            return View(modelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string TextToSearch)
        {
            return RedirectToAction(nameof(Index), new { Id = 1, SearchText = TextToSearch });
        }

        // GET: ProductController/Create
        public ActionResult Create(int pageNum, string SearchText)
        {
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            ProductModel_DTO_Create_Update model = new();
            return View(model);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductModel_DTO_Create_Update model, int pageNum, string SearchText)
        {
            var item = model.AsDaL();
            if (ModelState.IsValid && Manager.Processor.ValidateItem(item).IsValid)
            {
                await Manager.Processor.CreateItemAsync(item).ConfigureAwait(false);
                return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
            }
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            return View(nameof(Create), model);
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int Id, int pageNum, string SearchText)
        {
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            ViewData["ItemId"] = Id;
            var model = await Manager.Processor.LoadSingleItemAsync(Id);
            var m = model.AsDto();
            return View(m);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductModel_DTO_Create_Update model, int pageNum, string SearchText)
        {
            var item = model.AsDaL();
            if (ModelState.IsValid && Manager.Processor.ValidateItem(item).IsValid)
            {
                await Manager.Processor.UpdateItemAsync(item).ConfigureAwait(false);
                return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
            }
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            return View(nameof(Edit), model);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int Id, int pageNum, string SearchText)
        {
            await Manager.Processor.DeleteItemByIdAsync(Id);
            return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
        }
    }
}