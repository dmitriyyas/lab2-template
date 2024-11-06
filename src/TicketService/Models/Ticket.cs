using System;
using System.Collections.Generic;

namespace TicketService.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public Guid TicketUid { get; set; }

    public string Username { get; set; } = null!;

    public string FlightNumber { get; set; } = null!;

    public int Price { get; set; }

    public string Status { get; set; } = null!;
}
