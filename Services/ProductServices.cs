using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using blazorWebApplication.Models;

namespace blazorWebApplication.Services;

public class ProductService: IProductService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options;

    public ProductService(HttpClient client)
    {
        this.client = client;
        this.options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
    }
    
    public async Task<List<Product>> Get()
    {
        var response = await client.GetAsync("products");
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content);    
        }
            
        var products = JsonSerializer.Deserialize<List<Product>>(content, options);
        if (products == null)
        {
            throw new ApplicationException("Error deserializing the product list.");
        }

        return products;
        
    }

    public async Task Add(Product product)
    {
            var response = await client.PostAsync("products", JsonContent.Create(product));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
    }

    public async Task Update(Product product)
    {
        var content = new StringContent(JsonSerializer.Serialize(product, options), Encoding.UTF8, "application/json");
        await client.PutAsync($"products/{product.Id}", content);
    }

    public async Task Delete(int id)
    {
        var response = await client.DeleteAsync($"products/{id}");
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content);
        }
    }
}


public interface IProductService
{
    Task<List<Product>> Get();
    Task Add(Product product);
    Task Update(Product product);
    Task Delete(int id);
}