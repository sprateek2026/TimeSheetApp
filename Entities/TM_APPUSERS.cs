namespace Timesheet.Api.Entities
{
    public class TM_APPUSERS
    {
        public int ID { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string LOGIN { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD_HASH { get; set; }
        public int DESIGNATIONID { get; set; }
        public int DISPLAYDESIGNATIONID { get; set; }
        public int DEPARTMENTID { get; set; }
        public string USERROLE { get; set; }
        public int? MANAGERID { get; set; }
        public string EMPTYPE { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime? LASTLOGINON { get; set; }
        public DateTime HIREDATE { get; set; }
        public int? CREATEDBY { get; set; }
        // --------------------
        // Navigation Properties
        // --------------------
        public TM_APPUSERS Manager { get; set; }
        public ICollection<TM_APPUSERS> Subordinates { get; set; } = new List<TM_APPUSERS>();
        public ICollection<TM_DAILYSUMMARY> DailySummaries { get; set; } = new List<TM_DAILYSUMMARY>();
    }
}
