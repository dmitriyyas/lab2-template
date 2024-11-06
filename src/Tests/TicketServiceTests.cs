using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService;
using TicketService.Controllers;
using TicketService.Dto;
using TicketService.Models;

namespace Tests;

public class TicketServiceTests
{
    [Fact]
    public async Task TestBuyTicket_Ok()
    {
        var controller = new TicketController(DbHelper.CreateContext<TicketsContext>());

        var number = "test";
        var price = 1000;
        var name = "user";

        var ticketCreate = new TicketCreateDto(number, price);

        var response = (ObjectResult) await controller.BuyTicket(name, ticketCreate);

        var ticket = (TicketDto)response.Value;

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(price, ticket.Price);
        Assert.Equal(number, ticket.FlightNumber);
        Assert.Equal("PAID", ticket.Status);
    }

    [Fact]
    public async Task TestCancelTicket_Ok()
    {
        var context = DbHelper.CreateContext<TicketsContext>();
        var controller = new TicketController(context);

        var number = "test";
        var price = 1000;
        var name = "user";
        var uuid = Guid.NewGuid();

        var ticket = new Ticket { Id = 1, FlightNumber = number, TicketUid = uuid, Price = price, Status = "PAID", Username = name };
        await context.AddAsync(ticket);
        await context.SaveChangesAsync();

        var response = (NoContentResult) await controller.CancelTicket(name, uuid);

        var canceledTicket = await context.Tickets.FirstAsync(t => t.TicketUid == uuid);

        Assert.Equal(204, response.StatusCode);
        Assert.Equal("CANCELED", canceledTicket.Status);
    }

    [Fact]
    public async Task TestCancelTicket_Forbidden()
    {
        var context = DbHelper.CreateContext<TicketsContext>();
        var controller = new TicketController(context);

        var number = "test";
        var price = 1000;
        var name = "user";
        var badname = "user1";
        var uuid = Guid.NewGuid();

        var ticket = new Ticket { Id = 1, FlightNumber = number, TicketUid = uuid, Price = price, Status = "PAID", Username = name };
        await context.AddAsync(ticket);
        await context.SaveChangesAsync();

        var response = (ObjectResult)await controller.CancelTicket(badname, uuid);

        Assert.Equal(403, response.StatusCode);
    }

    [Fact]
    public async Task TestCancelTicket_NotFound()
    {
        var context = DbHelper.CreateContext<TicketsContext>();
        var controller = new TicketController(context);

        var response = (NotFoundObjectResult)await controller.CancelTicket("any", Guid.NewGuid());

        Assert.Equal(404, response.StatusCode);
    }
}
