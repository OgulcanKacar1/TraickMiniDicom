using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Models;
using TraickMiniDicom.Services;

namespace TraickMiniDicom.Data;

public class AppDbContext : DbContext
{
    // Artık o karmaşık HttpContext yerine kendi eğittiğimiz sekreteri alıyoruz!
    private readonly ICurrentUser _currentUser;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }
    
    public DbSet<Organization> Organizations { get; set; } // YENİ TABLOMUZU EKLEDİK
    public DbSet<Study> Studies { get; set; }
    public DbSet<StudyFile> StudyFiles { get; set; }
    public DbSet<User> Users { get; set; }

    // İŞTE BÜYÜ BURADA GERÇEKLEŞİYOR (Global Query Filter)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kural: Eğer sorguyu atan kişi "Admin" değilse, SADECE kendi hastanesine (OrganizationId) ait olan Çalışmaları çek!
        modelBuilder.Entity<Study>().HasQueryFilter(s => 
            _currentUser.Role == "Admin" || s.OrganizationId == _currentUser.OrganizationId);
            
        // Aynı kural Kullanıcı listesi için de geçerli (Böylece A hastanesi, B hastanesindeki doktorları göremez)
        modelBuilder.Entity<User>().HasQueryFilter(u => 
            _currentUser.Role == "Admin" || u.OrganizationId == _currentUser.OrganizationId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Uzun uzun kodlar yazmak yerine kendi Sekreterimizden direkt ID'yi alıyoruz
        // (Sisteme giriş yapmamış biri Register oluyorsa hata vermesin diye ufak bir try-catch koyduk)
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