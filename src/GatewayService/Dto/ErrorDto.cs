using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class ErrorDto(string message)
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = message;
}
