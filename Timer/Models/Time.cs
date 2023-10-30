namespace Timer.Models;

public class Time
{
    public int Id { get; set; }
    public string? Hours {  get; set; }
    public string? Minutes { get; set; }
    public string Seconds { get; set; }

    public string? Description { get; set;}

    public User? User { get; set; }
}
