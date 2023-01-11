using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class LabelModel
{
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; }= string.Empty;
    [JsonPropertyName("@default")]
    public bool Default { get; set; }
}

