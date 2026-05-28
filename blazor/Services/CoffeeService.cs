using System.Net.Http.Json;
using blazor.Models;

namespace blazor.Services;

public class CoffeeService(HttpClient http)
{
    public Task<Coffee[]?> ListAsync() =>
        http.GetFromJsonAsync<Coffee[]>("/api/coffees");

    public async Task<Coffee?> AddAsync(Coffee coffee)
    {
        var response = await http.PostAsJsonAsync("/api/coffees", coffee);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Coffee>() : null;
    }
}
