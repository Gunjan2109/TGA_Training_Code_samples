using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using InventorySearchApi.Models;

namespace InventorySearchApi.Services;

public class SearchService
{
    private readonly string _indexName = "products";
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient _searchClient;

    public SearchService(IConfiguration config)
    {
        var endpoint = new Uri(config["SearchService:Endpoint"]!);
        var apiKey = new AzureKeyCredential(config["SearchService:ApiKey"]!);
        _indexClient = new SearchIndexClient(endpoint, apiKey);
        _searchClient = new SearchClient(endpoint, _indexName, apiKey);
    }

    public async Task CreateIndexIfNotExistsAsync()
    {
        var fieldBuilder = new FieldBuilder();
        var searchFields = fieldBuilder.Build(typeof(Product));
        var definition = new SearchIndex(_indexName, searchFields);

        await _indexClient.CreateOrUpdateIndexAsync(definition);
    }

    public async Task UploadProductsAsync(List<Product> products)
    {
        await _searchClient.UploadDocumentsAsync(products);
    }

    public async Task<List<Product>> SearchAsync(string query)
    {
        var options = new SearchOptions { IncludeTotalCount = true };
        var results = await _searchClient.SearchAsync<Product>(query, options);

        var items = new List<Product>();
        await foreach (var result in results.Value.GetResultsAsync())
        {
            items.Add(result.Document);
        }
        return items;
    }

    public async Task<List<string>> SuggestAsync(string term)
    {
        var results = await _searchClient.SuggestAsync<Product>(term, "sg", new SuggestOptions());
        return results.Value.Results.Select(r => r.Text).ToList();
    }
}