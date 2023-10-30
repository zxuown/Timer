using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Timer.Models;

public class SiteContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public SiteContext(DbContextOptions options) : base(options)
    { }
    public DbSet<Time> Timers { get; set; } = null!;

    public DbSet<TimerToDate> TimersToDates { get; set; } = null!;
}
