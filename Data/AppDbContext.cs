using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Models;
using TraickMiniDicom.Services;

namespace TraickMiniDicom.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }
    
    public DbSet<Organization> Organizations { get; set; } // YENİ TABLOMUZU EKLEDİK
    public DbSet<Study> Studies { get; set; }
    public DbSet<StudyFile> StudyFiles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Study>().HasQueryFilter(s => 
            _currentUser.Role == "Admin" || s.OrganizationId == _currentUser.OrganizationId);
            
        modelBuilder.Entity<User>().HasQueryFilter(u => 
            _currentUser.Role == "Admin" || u.OrganizationId == _currentUser.OrganizationId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {

        Guid? userId = null;
        try { userId = _currentUser.UserId; } catch { }
        
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.IsDeleted = false;
                entry.Entity.CreatedBy = userId;
                
                if (entry.Entity.Id == Guid.Empty)
                    entry.Entity.Id = Guid.NewGuid();
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = userId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}