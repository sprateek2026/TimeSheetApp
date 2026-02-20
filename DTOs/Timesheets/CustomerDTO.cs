using Timesheet.Api.Enums;

namespace Timesheet.Api.DTOs.Timesheets
{
    public class CustomerDTO
    {
        public int id { get; set; }
        public string custcode { get; set; }
        public string billingaddress { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
