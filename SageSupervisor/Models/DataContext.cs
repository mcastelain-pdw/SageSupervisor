using Microsoft.EntityFrameworkCore;
using SageSupervisor.Models.DTO;

namespace SageSupervisor.Models;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<TableChangeDto> TableChangeDtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
