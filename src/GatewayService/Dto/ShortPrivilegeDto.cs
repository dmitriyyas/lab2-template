using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class ShortPrivilegeDto(int balance,
    string status)
{
    [JsonPropertyName("balance")]
    public int Balance { get; set; } = balance;

    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
}
