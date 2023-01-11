using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class SearchResultModel
{
    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    [JsonPropertyName("incomplete_results")]
    public bool IncompleteResults { get; set; }

    [JsonPropertyName("items")]
    public List<SearchResultItemModel> Items { get; set; } = new List<SearchResultItemModel>();
}

