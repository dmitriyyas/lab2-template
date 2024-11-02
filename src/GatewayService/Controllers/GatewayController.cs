using GatewayService.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("")]
public class GatewayController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private IHttpClientFactory _httpClientFactory = httpClientFactory;

    private readonly Uri BonusServiceUri = new("http://bonus_service:8050/");
    private readonly Uri FlightServiceUri = new("http://flight_service:8060/");
    private readonly Uri TicketServiceUri = new("http://ticket_service:8070/");

    [HttpGet("manage/health")]
    public IActionResult Health()
    {
        return Ok();
    }

    [HttpGet("api/v1/flights")]
    public async Task<IActionResult> GetFlights([FromQuery] int? page = null, [FromQuery] int? size = null)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = FlightServiceUri;

        var query = "api/v1/flights";
        if (page is not null && size is not null)
            query += $"?page={page}&size={size}";

        var flights = await client.GetFromJsonAsync<FlightsDto>(query);
        return Ok(flights);
    }

    [HttpGet("api/v1/me")]
    public async Task<IActionResult> GetUserInfo([FromHeader(Name = "X-User-Name")] string username)
    {
        var client = _httpClientFactory.CreateClient();

        using var ticketsRequest = new HttpRequestMessage(HttpMethod.Get, TicketServiceUri + "api/v1/tickets");
        ticketsRequest.Headers.Add("X-User-Name", username);

        using var ticketsResponse = await client.SendAsync(ticketsRequest);
        var tickets = await ticketsResponse.Content.ReadFromJsonAsync<List<TicketDto>>();

        var query = "api/v1/flights?" + string.Join('&', tickets.Select(t => $"numbers={t.FlightNumber}"));
        var flightRequest = new HttpRequestMessage(HttpMethod.Get, FlightServiceUri + query);

        using var flightResponse = await client.SendAsync(flightRequest);
        var flights = await flightResponse.Content.ReadFromJsonAsync<FlightsDto>();

        var ticketDetails = tickets.Select(ticket =>
        {
            var flight = flights.Items.First(f => f.FlightNumber == ticket.FlightNumber);
            return new TicketDetailDto(ticket.TicketUid,
                ticket.FlightNumber,
                flight.FromAirport,
                flight.ToAirport,
                flight.Date,
                ticket.Price,
                ticket.Status);
        }).ToList();

        using var privilegeRequest = new HttpRequestMessage(HttpMethod.Get, BonusServiceUri + "api/v1/privileges");
        privilegeRequest.Headers.Add("X-User-Name", username);

        using var privilegeResponse = await client.SendAsync(privilegeRequest);
        var privilege = await privilegeResponse.Content.ReadFromJsonAsync<PrivilegeDto>();

        var shortPrivilege = new ShortPrivilegeDto(privilege.Balance, privilege.Status);
        var userInfo = new UserInfoDto(ticketDetails, shortPrivilege);

        return Ok(userInfo);
    }

    [HttpGet("api/v1/tickets")]
    public async Task<IActionResult> GetTickets([FromHeader(Name = "X-User-Name")] string username)
    {
        var client = _httpClientFactory.CreateClient();

        using var ticketsRequest = new HttpRequestMessage(HttpMethod.Get, TicketServiceUri + "api/v1/tickets");
        ticketsRequest.Headers.Add("X-User-Name", username);

        using var ticketsResponse = await client.SendAsync(ticketsRequest);
        var tickets = await ticketsResponse.Content.ReadFromJsonAsync<List<TicketDto>>();

        var query = "api/v1/flights?" + string.Join('&', tickets.Select(t => $"numbers={t.FlightNumber}"));
        var flightRequest = new HttpRequestMessage(HttpMethod.Get, FlightServiceUri + query);

        using var flightResponse = await client.SendAsync(flightRequest);
        var flights = await flightResponse.Content.ReadFromJsonAsync<FlightsDto>();

        var ticketDetails = tickets.Select(ticket =>
        {
            var flight = flights.Items.First(f => f.FlightNumber == ticket.FlightNumber);
            return new TicketDetailDto(ticket.TicketUid,
                ticket.FlightNumber,
                flight.FromAirport,
                flight.ToAirport,
                flight.Date,
                ticket.Price,
                ticket.Status);
        }).ToList();

        return Ok(ticketDetails);
    }

    [HttpGet("api/v1/privilege")]
    public async Task<IActionResult> GetPrivilege([FromHeader(Name = "X-User-Name")] string username)
    {
        var client = _httpClientFactory.CreateClient();

        using var privilegeRequest = new HttpRequestMessage(HttpMethod.Get, BonusServiceUri + "api/v1/privileges");
        privilegeRequest.Headers.Add("X-User-Name", username);

        using var privilegeResponse = await client.SendAsync(privilegeRequest);
        var privilege = await privilegeResponse.Content.ReadFromJsonAsync<PrivilegeDto>();

        return Ok(privilege);
    }

    [HttpGet("api/v1/tickets/{ticketUid}")]
    public async Task<IActionResult> GetTicket([FromHeader(Name = "X-User-Name")] string username, [FromRoute] Guid ticketUid)
    {
        var client = _httpClientFactory.CreateClient();

        using var ticketRequest = new HttpRequestMessage(HttpMethod.Get, TicketServiceUri + $"api/v1/tickets/{ticketUid}");
        ticketRequest.Headers.Add("X-User-Name", username);

        using var ticketResponse = await client.SendAsync(ticketRequest);
        if (!ticketResponse.IsSuccessStatusCode)
            return StatusCode((int)ticketResponse.StatusCode, new ErrorDto(await ticketResponse.Content.ReadAsStringAsync()));
        
        var ticket = await ticketResponse.Content.ReadFromJsonAsync<TicketDto>();

        var flightRequest = new HttpRequestMessage(HttpMethod.Get, FlightServiceUri + $"api/v1/flights/{ticket.FlightNumber}");

        using var flightResponse = await client.SendAsync(flightRequest);
        if (!flightResponse.IsSuccessStatusCode)
            return StatusCode((int)flightResponse.StatusCode, new ErrorDto(await flightResponse.Content.ReadAsStringAsync()));

        var flight = await flightResponse.Content.ReadFromJsonAsync<FlightDto>();

        var ticketDetail = new TicketDetailDto(ticket.TicketUid,
            ticket.FlightNumber,
            flight.FromAirport,
            flight.ToAirport,
            flight.Date,
            ticket.Price,
            ticket.Status);

        return Ok(ticketDetail);
    }

    [HttpDelete("api/v1/tickets/{ticketUid}")]
    public async Task<IActionResult> CancelTicket([FromHeader(Name = "X-User-Name")] string username, [FromRoute] Guid ticketUid)
    {
        var client = _httpClientFactory.CreateClient();

        using var ticketRequest = new HttpRequestMessage(HttpMethod.Delete, TicketServiceUri + $"api/v1/tickets/{ticketUid}");
        ticketRequest.Headers.Add("X-User-Name", username);

        using var ticketResponse = await client.SendAsync(ticketRequest);
        if (!ticketResponse.IsSuccessStatusCode)
            return StatusCode((int)ticketResponse.StatusCode, new ErrorDto(await ticketResponse.Content.ReadAsStringAsync()));

        using var privilegeRequest = new HttpRequestMessage(HttpMethod.Delete, BonusServiceUri + $"api/v1/privileges/{ticketUid}");
        privilegeRequest.Headers.Add("X-User-Name", username);

        using var privilegeResponse = await client.SendAsync(privilegeRequest);
        if (!privilegeResponse.IsSuccessStatusCode)
            return StatusCode((int)privilegeResponse.StatusCode, new ErrorDto(await privilegeResponse.Content.ReadAsStringAsync()));

        return NoContent();
    }

    [HttpPost("api/v1/tickets")]
    public async Task<IActionResult> BuyTicket([FromHeader(Name = "X-User-Name")] string username, PurchaseRequestDto purchaseRequest)
    {
        var client = _httpClientFactory.CreateClient();

        var flightRequest = new HttpRequestMessage(HttpMethod.Get, FlightServiceUri + $"api/v1/flights/{purchaseRequest.FlightNumber}");

        using var flightResponse = await client.SendAsync(flightRequest);
        if (!flightResponse.IsSuccessStatusCode)
            return StatusCode((int)flightResponse.StatusCode, new ErrorDto(await flightResponse.Content.ReadAsStringAsync()));

        var flight = await flightResponse.Content.ReadFromJsonAsync<FlightDto>();

        var ticketRequest = new HttpRequestMessage(HttpMethod.Post, TicketServiceUri + $"api/v1/tickets");
        ticketRequest.Headers.Add("X-User-Name", username);
        ticketRequest.Content = JsonContent.Create(new TicketCreateDto(purchaseRequest.FlightNumber, purchaseRequest.Price));

        using var ticketResponse = await client.SendAsync(ticketRequest);
        if (!ticketResponse.IsSuccessStatusCode)
            return StatusCode((int)ticketResponse.StatusCode, new ErrorDto(await ticketResponse.Content.ReadAsStringAsync()));

        var ticket = await ticketResponse.Content.ReadFromJsonAsync<TicketDto>();

        using var buyRequest = new HttpRequestMessage(HttpMethod.Post, BonusServiceUri + $"api/v1/privileges");
        buyRequest.Headers.Add("X-User-Name", username);
        buyRequest.Content = JsonContent.Create(new TicketInfoDto(purchaseRequest.Price, purchaseRequest.PaidFromBalance, ticket.TicketUid, flight.Date));

        using var buyResponse = await client.SendAsync(buyRequest);
        if (!buyResponse.IsSuccessStatusCode)
            return StatusCode((int)buyResponse.StatusCode, new ErrorDto(await buyResponse.Content.ReadAsStringAsync()));

        var purchaseInfo = await buyResponse.Content.ReadFromJsonAsync<PurchaseInfoDto>();

        using var privilegeRequest = new HttpRequestMessage(HttpMethod.Get, BonusServiceUri + "api/v1/privileges");
        privilegeRequest.Headers.Add("X-User-Name", username);

        using var privilegeResponse = await client.SendAsync(privilegeRequest);
        var privilege = await privilegeResponse.Content.ReadFromJsonAsync<PrivilegeDto>();

        var shortPrivilege = new ShortPrivilegeDto(privilege.Balance, privilege.Status);

        var result = new PurchaseResponseDto(ticket.TicketUid,
            flight.FlightNumber,
            flight.FromAirport,
            flight.ToAirport,
            flight.Date,
            purchaseRequest.Price,
            purchaseInfo.PaidByMoney,
            purchaseInfo.PaidByBonuses,
            ticket.Status,
            shortPrivilege);

        return Ok(result);
    }
}
