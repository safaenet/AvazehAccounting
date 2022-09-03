using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System.Threading.Tasks;

namespace AvazehWeb.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppSettingsController : ControllerBase
    {
        public AppSettingsController(IAppSettingsManager manager)
        {
            Manager = manager;
        }

        private readonly IAppSettingsManager Manager;

        [HttpGet]
        public async Task<ActionResult<AppSettingsModel>> GetAllSettingsAsync()
        {
            var item = await Manager.LoadAllSettingsAsync();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.InvoiceSettings))]
        public async Task<ActionResult<InvoiceSettingsModel>> GetInvoiceSettingsAsync()
        {
            var item = await Manager.LoadInvoiceSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.TransactionSettings))]
        public async Task<ActionResult<TransactionSettingsModel>> GetTransactionSettingsAsync()
        {
            var item = await Manager.LoadTransactionSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.ChequeSettings))]
        public async Task<ActionResult<ChequeSettingsModel>> GetChequeSettingsAsync()
        {
            var item = await Manager.LoadChequeSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.GeneralSettings))]
        public async Task<ActionResult<GeneralSettingsModel>> GetGeneralSettingsAsync()
        {
            var item = await Manager.LoadGeneralSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.InvoicePrintSettings))]
        public async Task<ActionResult<InvoicePrintSettingsModel>> GetInvoicePrintSettingsAsync()
        {
            var item = await Manager.LoadInvoicePrintSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> SaveAllSettingsAsync(AppSettingsModel Settings)
        {
            return await Manager.SaveAllSettingsAsync(Settings);
        }
    }
}