using Microsoft.EntityFrameworkCore;
using TraickMiniDicom.Models;

namespace TraickMiniDicom.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Study> Studies { get; set; }
    public DbSet<StudyFile> StudyFiles { get; set; }
}