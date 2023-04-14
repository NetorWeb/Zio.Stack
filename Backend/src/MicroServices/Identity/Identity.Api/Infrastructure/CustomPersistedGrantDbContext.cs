using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure
{
    public class CustomPersistedGrantDbContext : PersistedGrantDbContext<CustomPersistedGrantDbContext>
    {
        public CustomPersistedGrantDbContext(DbContextOptions options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceFlowCodes>().Property(d => d.Data).HasColumnType("text");
            modelBuilder.Entity<PersistedGrant>().Property(d => d.Data).HasColumnType("text");
        }
    }
}