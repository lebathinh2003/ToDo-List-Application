using System.Text.Json.Serialization;
namespace WpfTaskManagerApp.Models;

public class PaginationMetadata
{
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalRow")]
    public int TotalRow { get; set; } // API của bạn trả về totalRow, không phải totalItems

    [JsonPropertyName("totalPage")]
    public int TotalPage { get; set; }
}

public class PaginatedResult<T>
{
    [JsonPropertyName("paginatedData")]
    public List<T> PaginatedData { get; set; } = new List<T>();

    [JsonPropertyName("metadata")]
    public PaginationMetadata? Metadata { get; set; }
}
