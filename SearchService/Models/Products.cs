using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace InventorySearchApi.Models;

public class Product
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [SearchableField(IsSortable = true)]
    public string Name { get; set; } = string.Empty;
}