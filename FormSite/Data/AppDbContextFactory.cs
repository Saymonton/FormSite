using System.IO;
using FormSite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FormSite.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Carrega configuração para o design-time (dotnet ef)
        var basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            // lê appsettings.json (se existir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            // lê variáveis de ambiente (ConnectionStrings__Default, etc.)
            .AddEnvironmentVariables()
            .Build();

        var connStr = config.GetConnectionString("Default")
                     ?? config["ConnectionStrings:Default"]
                     // fallback DEV para não travar (ajuste conforme seu ambiente)
                     ?? "Host=localhost;Port=5432;Database=formsitedb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connStr);

        return new AppDbContext(optionsBuilder.Options);
    }
}
