using System.Text;
using System.Text.Json;
using blazorWebApplication.Models;

namespace blazorWebApplication.Services;

public class CategoryService: ICategoryService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options;

    public CategoryService(HttpClient client)
    {
        this.client = client;
        this.options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
    }
    
    public async Task<List<Category>> Get()
    {
        var response = await client.GetAsync("categories");
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content);            
        }
        var categories = JsonSerializer.Deserialize<List<Category>>(content, options);
        if (categories == null)
        {
            throw new ApplicationException("Error deserializing the category list.");
        }
        return categories;

   }

    public async Task Add(Category category)
    {
        var content = new StringContent(JsonSerializer.Serialize(category, options), Encoding.UTF8, "application/json");
        await client.PostAsync("categories", content);
    }

    public async Task Update(Category category)
    {
        var content = new StringContent(JsonSerializer.Serialize(category, options), Encoding.UTF8, "application/json");
        await client.PutAsync($"categories/{category.Id}", content);
    }

    public async Task Delete(int id)
    {
        await client.DeleteAsync($"categories/{id}");
    }
}


public interface ICategoryService
{
    Task<List<Category>> Get();
    Task Add(Category category);
    Task Update(Category category);
    Task Delete(int id);
}