using System.Security.Cryptography;
using System.Text.Json;
using Coffee2Live.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Coffee2Live.App.Controllers;

/// <summary>
/// Controller for coffees
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CoffeesController : ControllerBase
{
    private readonly object _saveLock = new();
    private readonly Lazy<List<Coffee>> _coffees;
    private readonly string _dataPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoffeesController"/> class.
    /// </summary>
    public CoffeesController(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "coffees.json");
        _coffees = new Lazy<List<Coffee>>(() => LoadCoffees(_dataPath));
    }

    /// <summary>
    /// Get all coffees
    /// </summary>
    /// <returns>A list of all coffees </returns>
    [HttpGet]
    public ActionResult<IEnumerable<Coffee>> GetAll()
    {
        return Ok(_coffees.Value);
    }

    /// <summary>
    /// Get coffee by id
    /// </summary>
    /// <param name="id">The ID of the coffee to retrieve</param>
    /// <returns>The coffee with the specified ID, or a 404 if not found</returns>
    [HttpGet("{id}")]
    public ActionResult<Coffee> GetById(Guid id)
    {
        var coffee = _coffees.Value.FirstOrDefault(c => c.Id == id);
        if (coffee == null) return NotFound();
        return Ok(coffee);
    }

    /// <summary>
    /// Add a new coffee to the catalog
    /// </summary>
    /// <param name="coffee">The coffee to add</param>
    /// <returns>The created coffee</returns>
    [HttpPost]
    public ActionResult<Coffee> Create(Coffee? coffee)
    {
        if (coffee == null)
        {
            return BadRequest();
        }

        if (coffee.Id == Guid.Empty)
        {
            coffee.Id = Guid.NewGuid();
        }

        lock (_saveLock)
        {
            _coffees.Value.Add(coffee);
            SaveCoffees(_coffees.Value, _dataPath);
        }

        return CreatedAtAction(nameof(GetById), new { id = coffee.Id }, coffee);
    }

    private static List<Coffee> LoadCoffees(string path)
    {
        if (!System.IO.File.Exists(path)) return new List<Coffee>();
        var json = System.IO.File.ReadAllText(path);
        var items = JsonSerializer.Deserialize<List<CoffeeDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<CoffeeDto>();

        var list = new List<Coffee>();
        foreach (var i in items)
        {
            var coffee = new Coffee
            {
                Id = i.Id ?? CreateDeterministicGuid(i.Name),
                Name = i.Name ?? string.Empty,
                Origin = i.Origin ?? string.Empty,
                TastingNotes = i.TastingNotes ?? string.Empty,
                Bitterness = i.Bitterness,
                Body = i.Body,
                BestFor = i.BestFor ?? string.Empty,
                Acidity = TryParseEnum(i.Acidity, Acidity.Medium),
                Roast = TryParseEnum(i.Roast, Roast.Medium),
                Price = i.Price
            };
            list.Add(coffee);
        }
        return list;
    }

    private static void SaveCoffees(List<Coffee> coffees, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var items = coffees.Select(coffee => new CoffeeDto
        {
            Id = coffee.Id,
            Name = coffee.Name,
            Origin = coffee.Origin,
            TastingNotes = coffee.TastingNotes,
            Bitterness = coffee.Bitterness,
            Acidity = coffee.Acidity.ToString(),
            Body = coffee.Body,
            Roast = coffee.Roast.ToString(),
            BestFor = coffee.BestFor,
            Price = coffee.Price
        }).ToList();

        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        System.IO.File.WriteAllText(path, json);
    }

    private static TEnum TryParseEnum<TEnum>(string? value, TEnum fallback) where TEnum : struct
    {
        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<TEnum>(value, true, out var parsed))
            return parsed;
        return fallback;
    }

    private static Guid CreateDeterministicGuid(string? name)
    {
        var normalized = (name ?? string.Empty).Trim().ToLowerInvariant();
        var bytes = System.Text.Encoding.UTF8.GetBytes(normalized);
        var hash = MD5.HashData(bytes); // 16 bytes
        return new Guid(hash);
    }

    private class CoffeeDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Origin { get; set; }
        public string? TastingNotes { get; set; }
        public int Bitterness { get; set; }
        public string? Acidity { get; set; }
        public int Body { get; set; }
        public string? Roast { get; set; }
        public string? BestFor { get; set; }
        public decimal Price { get; set; }
    }
}
