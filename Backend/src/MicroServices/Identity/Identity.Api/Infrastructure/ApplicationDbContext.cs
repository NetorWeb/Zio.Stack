using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Claims> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //mysql字段编辑
            builder.Entity<ApplicationUser>().Property(d => d.NormalizedUserName).HasMaxLength(128);
            builder.Entity<ApplicationRole>().Property(d => d.NormalizedName).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<int>>().Property(d => d.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<int>>().Property(d => d.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserToken<int>>().Property(d => d.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserToken<int>>().Property(d => d.Name).HasMaxLength(128);
        }
    }
}