using Microsoft.EntityFrameworkCore;
using WizardSoftTestService.Models.Entity;

namespace WizardSoftTestService.Db;

public class AppDbContext : DbContext
{
    public DbSet<Node> Nodes => Set<Node>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Parent)
                  .WithMany(x => x.Children)
                  .HasForeignKey(x => x.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.Data)
                  .IsRequired()
                  .HasMaxLength(200);
        });
    }
}