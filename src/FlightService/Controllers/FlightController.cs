using FlightService.Dto;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers;

[ApiController]
[Route("/")]
public class FlightController(FlightsContext context) : ControllerBase
{
    private readonly FlightsContext _context = context;

    [HttpGet("manage/health")]
    public IActionResult Health()
    {
        return Ok();
    }

    [HttpGet("api/v1/flights")]
    public async Task<IActionResult> GetFlights([FromQuery] List<string>? numbers = null, [FromQuery] int? page = null, [FromQuery] int? size = null)
    {
        IQueryable<Flight> query = _context.Flights;

        if (page is not null && size is not null)
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);

        if (numbers is not null)
            query = query.Where(f => numbers.Contains(f.FlightNumber));

        var totalElements = await query.CountAsync();
        var flights = await query.Include(f => f.FromAirport).Include(f => f.ToAirport).ToListAsync();

        var result = new FlightsDto(page ?? 1,
            size ?? totalElements,
            totalElements,
            flights.Select(f => ToDto(f)).ToList());

        return Ok(result);
    }

    [HttpGet("api/v1/flights/{flightNumber}")]
    public async Task<IActionResult> GetFlight([FromRoute] string flightNumber)
    {
        var flight = await _context.Flights
            .Include(f => f.FromAirport)
            .Include(f => f.ToAirport)
            .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);

        if (flight is null)
            return NotFound($"flight with flightNumber = {flightNumber} wasn't found");

        return Ok(ToDto(flight));
    }

    private static FlightDto ToDto(Flight flight)
    {
        return new FlightDto(flight.FlightNumber,
            $"{flight.FromAirport!.City} {flight.FromAirport!.Name!}",
            $"{ flight.ToAirport!.City} {flight.ToAirport!.Name!}",
            flight.Datetime,
            flight.Price);
    }
}
