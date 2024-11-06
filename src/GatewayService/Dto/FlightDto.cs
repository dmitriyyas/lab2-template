using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class FlightDto(string flightNumber,
    string fromAirport,
    string toAirport,
    DateTime date,
    int price)
{
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
}
