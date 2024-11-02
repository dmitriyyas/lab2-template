using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class UserInfoDto(List<TicketDetailDto> tickets,
    ShortPrivilegeDto privilege)
{
    [JsonPropertyName("tickets")]
    public List<TicketDetailDto> Tickets { get; set; } = tickets;

    [JsonPropertyName("privilege")]
    public ShortPrivilegeDto privilege { get; set; } = privilege;
}
