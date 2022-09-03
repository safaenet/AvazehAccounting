using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
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

        [HttpGet("{Section}")]
        public async Task<ActionResult<object>> GetSettingsAsync(string Section)
        {
            if (string.IsNullOrEmpty(Section)) return NotFound("Couldn't find specific Item");
            var item = await Manager.LoadSettings(Section);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet("InvoiceSettings")]
        public async Task<ActionResult<InvoiceSettingsModel>> GetInvoiceSettingsAsync()
        {
            var item = await Manager.LoadInvoiceSettings();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }
    }
}