﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Contract.Common;

public class CommonPaginatedMetadata
{
    [Required]
    [JsonProperty("totalPage")]
    public int TotalPage { get; set; } = 0;

}

public class AdvancePaginatedMetadata : CommonPaginatedMetadata
{
    [Required]
    [JsonProperty("hasNextPage")]
    public bool HasNextPage { get; set; } = true;
}

public class NumberedPaginatedMetadata : CommonPaginatedMetadata
{
    [Required]
    [JsonProperty("currentPage")]
    public int CurrentPage { get; set; }

    [JsonProperty("totalRow")]
    public int? TotalRow { get; set; }
}
