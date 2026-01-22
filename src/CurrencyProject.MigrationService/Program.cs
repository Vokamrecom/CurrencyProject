using CurrencyProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Ошибка: ConnectionString не найден в конфигурации.");
    Environment.Exit(1);
}

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseNpgsql(connectionString, npgsqlOptions => 
    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

using var context = new ApplicationDbContext(optionsBuilder.Options);

try
{
    Console.WriteLine("Применение миграций...");
    await context.Database.MigrateAsync();
    Console.WriteLine("Миграции успешно применены.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
    Environment.Exit(1);
}
