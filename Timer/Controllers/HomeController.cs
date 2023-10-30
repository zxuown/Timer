using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timer.Models;

namespace Timer.Controllers;

public class HomeController : Controller
{
   
    private readonly SiteContext _siteContext;
    public HomeController(SiteContext context)
    {
        _siteContext = context;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/home/timers")]
    public IActionResult Timers()
    {
        var user = _siteContext.Users.First(x => x.Id == int.Parse(User.FindFirst("Id").Value)); 
        List<Time> timers = new();
        if (user != null)
        {
            timers = _siteContext.Timers.Include(x => x.User).Where(x => x.User.Id == user.Id).ToList();
        }
        return PartialView("~/Views/Home/_Table.cshtml", timers);
    }

    [Authorize]
    [HttpPost("/Home/Create")]
    public IActionResult Create([FromBody] Time time)
    {
        var user = _siteContext.Users.First(x => x.Id == int.Parse(User.FindFirst("Id").Value));

        var existingTimer = _siteContext.Timers.Include(x=>x.User).FirstOrDefault(x =>
            x.Hours == time.Hours &&
            x.Minutes == time.Minutes &&
            x.Seconds == time.Seconds && 
            x.User.Id == user.Id
        );
        if (existingTimer != null || time.Hours.Length > 3 || int.Parse(time.Hours) > 24)
        {
            return BadRequest();
        }
        
        time.User = user;
        _siteContext.Add(time);
        _siteContext.SaveChanges();
        return Ok();
    }

    [HttpPost("/Home/Delete/{id}")]
    public IActionResult Delete(int id)
    {
        var timer = _siteContext.Timers.First(x => x.Id == id);
        _siteContext.Timers.Remove(timer);
        _siteContext.SaveChanges();
        return Ok();
    }

    [HttpGet("/home/timers-to-dates")]
    public List<TimerToDate> TimersToDates()
    {
        var user = _siteContext.Users.First(x => x.Id == int.Parse(User.FindFirst("Id").Value));
        List<TimerToDate> timers = new();
        if (user != null)
        {
            timers = _siteContext.TimersToDates.Include(x => x.User).Where(x => x.User.Id == user.Id).ToList();
        }
        return timers;
    }

    [Authorize]
    [HttpPost("/home/create-timers-to-dates")]
    public IActionResult CreateTimersToDates([FromBody] TimerToDate timer)
    {
        _siteContext.RemoveRange(_siteContext.TimersToDates.Where(x => x.Date < DateTime.Today).ToList());
        _siteContext.SaveChanges();
        var user = _siteContext.Users.First(x => x.Id == int.Parse(User.FindFirst("Id").Value));

        timer.User = user;
        _siteContext.Add(timer);
        _siteContext.SaveChanges();
        return Ok();
    }
    [HttpPost("/home/delete-timers-to-dates/{id}")]
    public IActionResult DeleteTimersToDates(int id)
    {
        var timer = _siteContext.TimersToDates.First(x => x.Id == id);
        _siteContext.TimersToDates.Remove(timer);
        _siteContext.SaveChanges();
        return Ok();
    }
}
