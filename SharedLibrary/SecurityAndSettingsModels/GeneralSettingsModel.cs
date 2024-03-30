using SharedLibrary.DalModels;
using System.Collections.Generic;

namespace SharedLibrary.SecurityAndSettingsModels;

public class GeneralSettingsModel
{
    public string CompanyName { get; set; } = "آوازه";
    public int BarcodeAddItemCount { get; set; } = 1;
    public bool CanHaveDuplicateItemsInInvoice { get; set; } = true;
}