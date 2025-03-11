using Microsoft.EntityFrameworkCore;
using SageSupervisor.Models.DTO;

namespace SageSupervisor.Models;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<DocumentChangeDto> DocumentChangeDtos { get; set; }
    public DbSet<TiersChangeDto> TiersChangeDtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
