using System.Text.Json.Serialization;

namespace BonusService.Dto;

public class PrivilegeDto(int balance,
    string status,
    List<PrivilegeHistoryDto> history)
{
    [JsonPropertyName("balance")]
    public int Balance { get; set; } = balance;

    [JsonPropertyName("status")]
    public string Status { get; set; } = status;

    [JsonPropertyName("history")]
    public List<PrivilegeHistoryDto> History { get; set; } = history;
}
