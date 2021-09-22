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
            var modelList = Logics.Mapper.MapProductModel(Manager.Items);
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
            Models.ProductModel_Dto model = new();
            return View(model);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Models.ProductModel_Dto model, int pageNum, string SearchText)
        {
            if (ModelState.IsValid)
            {
                var item = Logics.Mapper.MapProductModel(model);
                await Manager.Processor.CreateItemAsync(item).ConfigureAwait(false);
                return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
            }
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            return View(nameof(Create), model);
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int id, int pageNum, string SearchText)
        {
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            var model = await Manager.Processor.LoadSingleItemAsync(id);
            var m = Logics.Mapper.MapProductModel(model);
            return View(m);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Models.ProductModel_Dto model, int pageNum, string SearchText)
        {
            if (ModelState.IsValid)
            {
                var item = Logics.Mapper.MapProductModel(model);
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