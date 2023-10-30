namespace Timer.Models;

public class TimerToDate
{
    public int Id {  get; set; }

    public string Label { get; set; }
    public DateTime Date { get; set; }

    public virtual User User { get; set; }
}
