using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(r => r.Id).HasMaxLength(255);
                entity.Property(r => r.Name).HasMaxLength(255);
                entity.Property(r => r.NormalizedName).HasMaxLength(255);
            });

            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property(u => u.Id).HasMaxLength(255);
                entity.Property(u => u.UserName).HasMaxLength(255);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(255);
                entity.Property(u => u.Email).HasMaxLength(255);
                entity.Property(u => u.NormalizedEmail).HasMaxLength(255);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(l => l.LoginProvider).HasMaxLength(255);
                entity.Property(l => l.ProviderKey).HasMaxLength(255);
                entity.Property(l => l.UserId).HasMaxLength(255);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(t => t.LoginProvider).HasMaxLength(255);
                entity.Property(t => t.Name).HasMaxLength(255);
                entity.Property(t => t.UserId).HasMaxLength(255);
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(r => r.UserId).HasMaxLength(255);
                entity.Property(r => r.RoleId).HasMaxLength(255);
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(rc => rc.RoleId).HasMaxLength(255);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(uc => uc.UserId).HasMaxLength(255);
            });
        }
    }
}
