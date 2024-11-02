using System;
using System.Collections.Generic;

namespace FlightService.Models;

public partial class Flight
{
    public int Id { get; set; }

    public string FlightNumber { get; set; } = null!;

    public DateTime Datetime { get; set; }

    public int? FromAirportId { get; set; }

    public int? ToAirportId { get; set; }

    public int Price { get; set; }

    public virtual Airport? FromAirport { get; set; }

    public virtual Airport? ToAirport { get; set; }
}
