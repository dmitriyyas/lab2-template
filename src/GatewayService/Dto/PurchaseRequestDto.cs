using System.Text.Json.Serialization;

namespace GatewayService.Dto;

public class PurchaseRequestDto(string flightNumber,
    int price, 
    bool paidFromBalance)
{
    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; set; } = flightNumber;

    [JsonPropertyName("price")]
    public int Price { get; set; } = price;

    [JsonPropertyName("paidFromBalance")]
    public bool PaidFromBalance { get; set; } = paidFromBalance;
}
