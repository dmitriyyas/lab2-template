using System.Text.Json.Serialization;

namespace FlightService.Dto;

public class FlightsDto(int page,
    int pageSize,
    int totalElements,
    List<FlightDto> items)
{
    [JsonPropertyName("page")]
    public int Page { get; set; } = page;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = pageSize;

    [JsonPropertyName("totalElements")]
    public int TotalElements { get; set; } = totalElements;

    [JsonPropertyName("items")]
    public List<FlightDto> Items { get; set; } = items;
}
