using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class TicketInfoDto(int price,
    bool paidFromBalance,
    Guid ticketUid,
    DateTime date)
{
    [JsonPropertyName("price")]
    public int Price { get; set; } = price;

    [JsonPropertyName("paidFromBalance")]
    public bool PaidFromBalance { get; set; } = paidFromBalance;

    [JsonPropertyName("ticketUid")]
    public Guid TicketUid { get; set; } = ticketUid;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = date;
}
