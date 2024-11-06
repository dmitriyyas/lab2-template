using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class TicketDetailDto(Guid ticketUid,
    string flightNumber,
    string fromAirport,
    string toAirport,
    DateTime date,
    int price,
    string status)
{
    [JsonPropertyName("ticketUid")]
    public Guid TicketUid { get; set; } = ticketUid;

    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; set; } = flightNumber;

    [JsonPropertyName("fromAirport")]
    public string FromAirport { get; set; } = fromAirport;

    [JsonPropertyName("toAirport")]
    public string ToAirport { get; set; } = toAirport;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = date;

    [JsonPropertyName("price")]
    public int Price { get; set; } = price;

    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
}
