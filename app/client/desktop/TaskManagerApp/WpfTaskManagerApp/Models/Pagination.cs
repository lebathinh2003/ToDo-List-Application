using System.Text.Json.Serialization;
using System.Collections.Generic; // Added for List<T>

namespace WpfTaskManagerApp.Models;

// Pagination metadata.
public class PaginationMetadata
{
    // Current page number.
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    // Total number of rows.
    [JsonPropertyName("totalRow")]
    public int TotalRow { get; set; }

    // Total number of pages.
    [JsonPropertyName("totalPage")]
    public int TotalPage { get; set; }
}

// Generic paginated result.
public class PaginatedResult<T>
{
    // Data for the current page.
    [JsonPropertyName("paginatedData")]
    public List<T> PaginatedData { get; set; } = new List<T>();

    // Pagination details.
    [JsonPropertyName("metadata")]
    public PaginationMetadata? Metadata { get; set; }
}