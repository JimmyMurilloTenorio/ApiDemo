using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
               new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
               new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
           );
        }
    }



}
