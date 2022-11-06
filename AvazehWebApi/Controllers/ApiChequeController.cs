using AvazehWeb;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AvazehWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChequeController : ControllerBase
    {
        public ChequeController(IChequeCollectionManager manager, System.IServiceProvider service)
        {
            Manager = manager;
            Service = service;
        }

        private readonly IChequeCollectionManager Manager;
        private readonly IServiceProvider Service;

        //GET /Cheque?Id=1&SearchText=sometext
        [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewChequesList))]
        public async Task<ActionResult<ItemsCollection_DTO<ChequeModel>>> GetItemsAsync(int Page = 1, string SearchText = "", string OrderBy = "DueDate", OrderType orderType = OrderType.DESC, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, int PageSize = 50, bool ForceLoad = false)
        {
            Manager.GenerateWhereClause(SearchText, OrderBy, orderType, listQueryStatus);
            Manager.PageSize = PageSize;
            if (ForceLoad) Manager.Initialized = false;
            await Manager.GotoPageAsync(Page);
            if (Manager.Items == null || Manager.Items.Count == 0) return NotFound("List is empty");
            return Manager.AsDto();
        }

        [HttpGet("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewChequeDetails))]
        public async Task<ActionResult<ChequeModel>> GetItemAsync(int Id)
        {
            var item = await Manager.Processor.LoadSingleItemAsync(Id);
            if (item is null) return NotFound("Couldn't find specific Item");
            return item;
        }

        [HttpGet("Banknames"), Authorize]
        public async Task<ActionResult<List<string>>> GetBanknamesAsync()
        {
            var items = await Manager.Processor.GetBanknames();
            return items is null ? NotFound("Couldn't find any match") : items;
        }

        [HttpGet("CloseCheques/{Days}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanViewChequesList))]
        public async Task<ActionResult<List<ChequeModel>>> GetCloseChequesAsync(int Days)
        {
            int Id = int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber));
            IUserProcessor Processor;
            Processor = Service.GetService(typeof(IUserProcessor)) as IUserProcessor;
            var settings = await Processor.GetUserSettingsAsync(Id);
            var items = await Manager.Processor.LoadChequesByDueDate(PersianCalendarModel.GetCurrentRawPersianDate(), PersianCalendarModel.GetCurrentRawPersianDate(settings.ChequeNotifyDays));
            return items is null ? NotFound("Couldn't find any match") : items.ToList();
        }

        [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanAddNewCheque))]
        public async Task<ActionResult<ChequeModel>> CreateItemAsync(ChequeModel_DTO_Create_Update model)
        {
            var newItem = model.AsDaL();
            if (!Manager.Processor.ValidateItem(newItem).IsValid) return BadRequest(0);
            await Manager.Processor.CreateItemAsync(newItem);
            return newItem;
        }

        [HttpPut("{Id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanEditCheque))]
        public async Task<ActionResult<ChequeModel>> UpdateItemAsync(int Id, ChequeModel_DTO_Create_Update model)
        {
            if (model is null) return BadRequest("Model is not valid");
            var updatedModel = model.AsDaL();
            if (!Manager.Processor.ValidateItem(updatedModel).IsValid) return BadRequest("Model is not valid");
            updatedModel.Id = Id;
            if (await Manager.Processor.UpdateItemAsync(updatedModel) == 0) return NotFound();
            return updatedModel;
        }

        [HttpDelete, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserPermissionsModel.CanDeleteCheque))]
        public async Task<ActionResult> DeleteItemAsync(int Id)
        {
            if (await Manager.Processor.DeleteItemByIdAsync(Id) > 0) return Ok("Successfully deleted the item");
            return NotFound("Item not found");
        }
    }
}