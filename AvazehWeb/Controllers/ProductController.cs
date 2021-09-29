using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Threading.Tasks;

namespace AvazehWeb.Controllers
{
    public class ProductController : Controller
    {
        public ProductController(ICollectionManager<ProductModel, IProcessor<ProductModel>> manager)
        {
            Manager = manager;
        }
        private readonly ICollectionManager<ProductModel, IProcessor<ProductModel>> Manager;

        // GET: ProductController
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var CurrentTempPage = HttpContext.Session.GetInt32("CurrentPage");
            int CurrentPage = CurrentTempPage == null ? 1 : (int)CurrentTempPage;
            var SearchText = HttpContext.Session.GetString("TextToSearch");
            if (!Manager.Initialized || Manager.SearchValue != SearchText) Manager.GenerateWhereClause(SearchText, "ProductName", OrderType.ASC);
            if (Manager.CurrentPage != CurrentPage) await Manager.GotoPageAsync(CurrentPage);
            ViewData["PagesCount"] = Manager.PagesCount;
            var modelList = Manager.Items;
            return View(modelList);
        }

        [HttpGet]
        public ActionResult GotoPage(int Id)
        {
            HttpContext.Session.SetInt32("CurrentPage", Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string TextToSearch)
        {
            if (TextToSearch != null)
                HttpContext.Session.SetString("TextToSearch", TextToSearch);
            return RedirectToAction(nameof(GotoPage), new { Id = 1 });
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            ProductModel_DTO_Create_Update model = new();
            return View(model);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductModel_DTO_Create_Update model)
        {
            var item = model.AsDaL();
            if (ModelState.IsValid && Manager.Processor.ValidateItem(item).IsValid)
            {
                await Manager.Processor.CreateItemAsync(item).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            return View(nameof(Create), model);
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int Id)
        {
            ViewData["ItemId"] = Id;
            var model = await Manager.Processor.LoadSingleItemAsync(Id);
            var m = model.AsDto();
            return View(m);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductModel_DTO_Create_Update model, int Id)
        {
            var item = model.AsDaL();
            item.Id = Id;
            if (ModelState.IsValid && Manager.Processor.ValidateItem(item).IsValid)
            {
                await Manager.Processor.UpdateItemAsync(item).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = Id;
            return View(nameof(Edit), model);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int Id)
        {
            await Manager.Processor.DeleteItemByIdAsync(Id);
            return RedirectToAction(nameof(Index));
        }
    }
}