using CVTool.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CVTool.Data
{
    public class DataContext: DbContext
    {
        protected readonly IConfiguration Configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<ImageMetaData> ImageMetaDatas { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<ComponentEntry> ComponentEntries { get; set; }
        public DbSet<ComponentChildEntry> ComponentChildEntries { get; set; }

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var prodConnectionString = Configuration.GetConnectionString("WebApiDatabaseLocal");
            options.UseSqlServer(prodConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resume>().HasOne(u => u.Owner)
                .WithMany(u => u.Resumes).HasForeignKey(r => r.OwnerId);

            modelBuilder.Entity<UserSession>().HasKey(us => us.SessionId);

            modelBuilder.Entity<User>().OwnsMany(u => u.RefreshTokens)
                .WithOwner(rt => rt.User).HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<Resume>().HasMany(r => r.Components)
                .WithOne(c => c.Resunme).HasForeignKey(c => c.ResumeId);

            modelBuilder.Entity<Component>().HasMany(c => c.ComponentEntries)
                .WithOne(ce => ce.Component).HasForeignKey(ce => ce.ComponentId);

            modelBuilder.Entity<ComponentEntry>().HasMany(ce => ce.Children)
                .WithOne(ch => ch.ParentComponentEntry).HasForeignKey(ce => ce.ParentComponentEntryId);
        }
    }
}
