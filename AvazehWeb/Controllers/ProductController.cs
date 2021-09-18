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
        public ProductController(IProductCollectionManager pcm)
        {
            PCM = pcm;
        }
        private readonly IProductCollectionManager PCM;

        // GET: ProductController
        public ActionResult Index(int Id = 1, string SearchText = "")
        {
            if (!PCM.Initialized || PCM.SearchValue != SearchText) PCM.GenerateWhereClause(SearchText);
            if (PCM.CurrentPage != Id) PCM.GotoPage(Id);
            ViewData["CurrentPage"] = PCM.CurrentPage;
            ViewData["Search"] = SearchText;
            ViewData["PagesCount"] = PCM.PagesCount;
            var modelList = Logics.Mapper.MapProductModel(PCM.Items);
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
            Models.ProductModel model = new();
            return View(model);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.ProductModel model, int pageNum, string SearchText)
        {
            if (ModelState.IsValid)
            {
                var item = Logics.Mapper.MapProductModel(model);
                PCM.Processor.CreateItem(item);
                return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
            }
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            return View(nameof(Create), model);
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id, int pageNum, string SearchText)
        {
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            var model = PCM.Processor.LoadSingleItem(id);
            var m = Logics.Mapper.MapProductModel(model);
            return View(m);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ProductModel model, int pageNum, string SearchText)
        {
            if (ModelState.IsValid)
            {
                var item = Logics.Mapper.MapProductModel(model);
                PCM.Processor.UpdateItem(item);
                return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
            }
            ViewData["pageNum"] = pageNum;
            ViewData["Search"] = SearchText;
            return View(nameof(Edit), model);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id, int pageNum, string SearchText)
        {
            PCM.DeleteItemFromDbById(Id);
            return RedirectToAction(nameof(Index), new { Id = pageNum, SearchText });
        }
    }
}