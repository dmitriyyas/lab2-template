using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Controllers;
using TicketService;
using BonusService;
using BonusService.Controllers;
using BonusService.Dto;
using Microsoft.AspNetCore.Mvc;
using BonusService.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class BonusServiceTests
{
    [Fact]
    public async Task BuyTicket_FromZeroBalance()
    {
        var context = DbHelper.CreateContext<PrivilegesContext>();
        var controller = new PrivilegeController(context);

        var price = 1000;
        var name = "user";

        var info = new TicketInfoDto(price, true, Guid.NewGuid(), DateTime.Today);

        var response = (ObjectResult) await controller.BuyTicket(name, info);

        var purchase = (PurchaseInfoDto) response.Value;

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(0, purchase.PaidByBonuses);
        Assert.Equal(1000, purchase.PaidByMoney);
    }

    [Fact]
    public async Task BuyTicket_FromLowBalance()
    {
        var context = DbHelper.CreateContext<PrivilegesContext>();
        var controller = new PrivilegeController(context);

        var price = 1000;
        var balance = 600;
        var name = "user";

        var privilege = new Privilege { Id = 1, Balance = balance, Username = name, Status = "BRONZE" };
        await context.Privileges.AddAsync(privilege);
        await context.SaveChangesAsync();

        var info = new TicketInfoDto(price, true, Guid.NewGuid(), DateTime.Today);

        var response = (ObjectResult)await controller.BuyTicket(name, info);

        var purchase = (PurchaseInfoDto)response.Value;

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(balance, purchase.PaidByBonuses);
        Assert.Equal(price - balance, purchase.PaidByMoney);
    }

    [Fact]
    public async Task BuyTicket_FromHighBalance()
    {
        var context = DbHelper.CreateContext<PrivilegesContext>();
        var controller = new PrivilegeController(context);

        var price = 1000;
        var balance = 1600;
        var name = "user";

        var privilege = new Privilege { Id = 1, Balance = balance, Username = name, Status = "BRONZE" };
        await context.Privileges.AddAsync(privilege);
        await context.SaveChangesAsync();

        var info = new TicketInfoDto(price, true, Guid.NewGuid(), DateTime.Today);

        var response = (ObjectResult)await controller.BuyTicket(name, info);

        var purchase = (PurchaseInfoDto)response.Value;

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(price, purchase.PaidByBonuses);
        Assert.Equal(0, purchase.PaidByMoney);
    }

    [Fact]
    public async Task BuyTicket_AccumulateBalance()
    {
        var context = DbHelper.CreateContext<PrivilegesContext>();
        var controller = new PrivilegeController(context);

        var price = 1000;
        var balance = 1600;
        var name = "user";

        var privilege = new Privilege { Id = 1, Balance = balance, Username = name, Status = "BRONZE" };
        await context.Privileges.AddAsync(privilege);
        await context.SaveChangesAsync();

        var info = new TicketInfoDto(price, false, Guid.NewGuid(), DateTime.Today);

        var response = (ObjectResult)await controller.BuyTicket(name, info);

        var purchase = (PurchaseInfoDto)response.Value;

        var updatedPrivilege = await context.Privileges.FirstAsync(p => p.Username == name);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(0, purchase.PaidByBonuses);
        Assert.Equal(price, purchase.PaidByMoney);
        Assert.Equal(balance + (int)(0.1 * price), updatedPrivilege.Balance);
    }
}
