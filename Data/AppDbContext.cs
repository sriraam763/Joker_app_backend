using Joker_app_backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Joker_app_backend.Data;

public class AppDbContext:IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Joke> Jokes => Set<Joke>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Joke>()
            .HasOne(j => j.User)
            .WithMany(u => u.Jokes)
            .HasForeignKey(j => j.UserID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}