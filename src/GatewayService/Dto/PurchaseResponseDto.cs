namespace GatewayService.Dto;

public class PurchaseResponseDto(Guid ticketUid,
    string flightNumber,
    string fromAirport,
    string toAirport,
    DateTime date,
    int price,
    int paidByMoney,
    int paidByBonuses,
    string status,
    ShortPrivilegeDto privilege)
{
    public Guid TicketUid { get; set; } = ticketUid;
    public string FlightNumber { get; set; } = flightNumber;
    public string FromAirport { get; set; } = fromAirport;
    public string ToAirport { get; set; } = toAirport;
    public DateTime Date { get; set; } = date;
    public int Price { get; set; } = price;
    public int PaidByMoney { get; set; } = paidByMoney;
    public int PaidByBonuses { get; set; } = paidByBonuses;
    public string Status { get; set; } = status;
    public ShortPrivilegeDto Privilege { get; set; } = privilege;
}
