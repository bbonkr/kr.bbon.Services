using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class UpdateIssueRequestModel : CreateIssueRequestModel, IIssueNumberModel
{
    [JsonIgnore]
    public long IssueNumber { get; set; }

    /// <summary>
    /// State of the issue. Either open or closed. 
    /// Can be one of: open, closed
    /// <para>
    /// See <see cref="IssueStates" /> fileds.
    /// </para>
    /// </summary>
    public string State { get; set; }= string.Empty;

    /// <summary>
    /// The reason for the current state    
    /// Can be one of: completed, not_planned, reopened,
    /// 
    /// See <see cref="IssueStateReasons" /> fields
    /// </summary>
    [JsonPropertyName("state_reason")]
    public string? StateReason { get; set; }
}
