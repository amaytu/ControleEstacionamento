using Microsoft.EntityFrameworkCore;

public class ParkingContext : DbContext
{
    public DbSet<TabelaDePreco> TabelasDePrecos { get; set; }
    public DbSet<RegistroEstacionamento> RegistrosEstacionamento { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=parking.db");
    }
}