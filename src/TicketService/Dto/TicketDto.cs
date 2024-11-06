using System.Text.Json.Serialization;

namespace TicketService.Dto;

public class TicketDto(Guid ticketUid,
    string flightNumber,
    int price,
    string status)
{
    [JsonPropertyName("ticketUid")]
    public Guid TicketUid { get; set; } = ticketUid;

    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; set; } = flightNumber;

    [JsonPropertyName("price")]
    public int Price { get; set; } = price;

    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
}
