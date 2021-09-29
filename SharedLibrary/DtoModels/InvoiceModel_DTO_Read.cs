using SharedLibrary.DalModels;

namespace SharedLibrary.DtoModels
{
    public class InvoiceModel_DTO_Read
    {
        public InvoiceModel Invoice { get; set; }
        public double CustomerTotalBalance { get; set; }
    }
}