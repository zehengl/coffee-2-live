namespace blazor.Models;

public class Coffee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string TastingNotes { get; set; } = string.Empty;
    public int Bitterness { get; set; }
    public int Acidity { get; set; }
    public int Body { get; set; }
    public int Roast { get; set; }
    public string BestFor { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
