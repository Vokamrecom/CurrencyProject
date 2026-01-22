using CurrencyProject.Domain.Entities;
using CurrencyProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyProject.CurrencyUpdater;

public class CurrencyUpdateWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CurrencyUpdateWorker> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(1);

    public CurrencyUpdateWorker(
        IServiceProvider serviceProvider,
        ILogger<CurrencyUpdateWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateCurrenciesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении курсов валют");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }
    }

    private async Task UpdateCurrenciesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            _logger.LogInformation("Начало обновления курсов валют");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var client = new HttpClient();
            var responseBytes = await client.GetByteArrayAsync("http://www.cbr.ru/scripts/XML_daily.asp");
            
            // Декодируем ответ с учетом кодировки windows-1251
            var encoding = Encoding.GetEncoding("windows-1251");
            var response = encoding.GetString(responseBytes);

            var serializer = new XmlSerializer(typeof(ValCurs));
            using var reader = new StringReader(response);
            var valCurs = (ValCurs)serializer.Deserialize(reader)!;

            foreach (var valute in valCurs.Valutes)
            {
                var valueStr = valute.Value.Replace(",", ".");
                var rate = decimal.Parse(valueStr, CultureInfo.InvariantCulture) / valute.Nominal;

                var currency = await context.Currencies
                    .FirstOrDefaultAsync(c => c.Name == valute.CharCode);

                if (currency != null)
                {
                    currency.Rate = rate;
                }
                else
                {
                    currency = new Currency
                    {
                        Name = valute.CharCode,
                        Rate = rate
                    };
                    context.Currencies.Add(currency);
                }
            }

            await context.SaveChangesAsync();
            _logger.LogInformation("Курсы валют успешно обновлены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении курсов валют");
            throw;
        }
    }
}

[XmlRoot("ValCurs")]
public class ValCurs
{
    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; } = new();
}

public class Valute
{
    [XmlElement("CharCode")]
    public string CharCode { get; set; } = string.Empty;

    [XmlElement("Value")]
    public string Value { get; set; } = string.Empty;

    [XmlElement("Nominal")]
    public int Nominal { get; set; }
}
