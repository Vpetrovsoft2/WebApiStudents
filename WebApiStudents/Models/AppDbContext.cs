using Microsoft.EntityFrameworkCore;

namespace WebApiStudents.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Student>? Students { get; set; }
    public DbSet<Facultet>? Facultes { get; set; }
}
