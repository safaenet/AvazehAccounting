using DataLibraryCore.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.SecurityAndSettingsModels;
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

        [HttpGet, Authorize]
        public async Task<ActionResult<AppSettingsModel>> GetAllSettingsAsync()
        {
            var item = await Manager.LoadAllSettingsAsync();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.GeneralSettings)), Authorize]
        public async Task<ActionResult<GeneralSettingsModel>> GetGeneralSettingsAsync()
        {
            var item = await Manager.LoadGeneralSettingsAsync();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet(nameof(AppSettingsModel.PrintSettings)), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{nameof(UserPermissionsModel.CanPrintInvoice)}, {nameof(UserPermissionsModel.CanPrintTransaction)}")]
        public async Task<ActionResult<PrintSettingsModel>> GetInvoicePrintSettingsAsync()
        {
            var item = await Manager.LoadPrintSettingsAsync();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanManageOthers))]
        public async Task<ActionResult<bool>> SaveAllSettingsAsync(AppSettingsModel Settings)
        {
            return await Manager.SaveAllSettingsAsync(Settings);
        }
    }
}