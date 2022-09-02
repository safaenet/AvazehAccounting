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
        public async Task<ActionResult<AppSettingsModel>> GetItemAsync()
        {
            var item = await Manager.LoadAllSettingsAsync();
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }
    }
}