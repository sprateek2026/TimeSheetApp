using Timesheet.Api.Entities;

public class TM_COMMENTS
{
    public int ID { get; set; }
    public int SUMMARYID { get; set; }  
    public string COMMENTTEXT { get; set; } = string.Empty;
    public int COMMENTBY { get; set; }
    //public DailySummary Summary { get; set; } = null!;
    //public AppUser User { get; set; } = null!;
    //public DateTime CommentDate { get; set; }
}
