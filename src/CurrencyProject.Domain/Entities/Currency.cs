namespace CurrencyProject.Domain.Entities;

public class Currency
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}
