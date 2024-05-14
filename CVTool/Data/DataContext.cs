using CVTool.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVTool.Data
{
    public class DataContext: DbContext
    {
        protected readonly IConfiguration Configuration;
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ImageMetaData> ImageMetaDatas { get; set; }
        public virtual DbSet<Resume> Resumes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<ComponentEntry> ComponentEntries { get; set; }
        public virtual DbSet<ComponentChildEntry> ComponentChildEntries { get; set; }

        public DataContext()
        {
        }
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
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
