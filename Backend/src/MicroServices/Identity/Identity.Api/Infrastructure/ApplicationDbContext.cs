using Identity.Api.Configuration;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var ph = new MyPasswordHasher();

            builder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
            builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                RealName = "alice1",
                UserName = "alice1",
                PasswordHash = ph.HashPassword(null, "123456")
            });

            #region 初始化用戶与角色的种子数据
            //1. 更新用戶与角色的外鍵
            builder.Entity<ApplicationUser>(
                u => u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired()
                );
            //2. 添加管理员角色
            var adminRoleId = "f8df1775-e889-46f4-acdd-421ec8d9ba64";
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            );
            //3. 添加用户
            var adminUserId = "f8df1775-e889-46f4-acdd-421ec8d9ba65";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "admin".ToUpper(),
                RealName = "admin",
                NormalizedEmail = "admin@qq.com".ToUpper(),
                Email = "admin@qq.com",
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false,
            };
            adminUser.PasswordHash = ph.HashPassword(adminUser, "123456");

            builder.Entity<ApplicationUser>().HasData(adminUser);
            //4. 给用户加入管理员角色
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
                );
            #endregion
        }
    }
}
