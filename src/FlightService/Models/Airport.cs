using System;
using System.Collections.Generic;

namespace FlightService.Models;

public partial class Airport
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Flight> FlightFromAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightToAirports { get; set; } = new List<Flight>();
}
