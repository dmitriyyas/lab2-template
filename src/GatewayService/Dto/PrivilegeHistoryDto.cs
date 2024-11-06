using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class PrivilegeHistoryDto(DateTime date,
    Guid ticketUid,
    int balanceDiff,
    string operationType)
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = date;

    [JsonPropertyName("ticketUid")]
    public Guid TicketUid { get; set; } = ticketUid;

    [JsonPropertyName("balanceDiff")]
    public int BalanceDiff { get; set; } = balanceDiff;

    [JsonPropertyName("operationType")]
    public string OperationType { get; set; } = operationType;
}
