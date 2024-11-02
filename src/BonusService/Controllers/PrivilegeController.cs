using BonusService.Dto;
using BonusService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonusService.Controllers;

[ApiController]
[Route("/")]
public class PrivilegeController(PrivilegesContext context) : ControllerBase
{
    private readonly PrivilegesContext _context = context;

    [HttpGet("manage/health")]
    public IActionResult Health()
    {
        return Ok();
    }

    [HttpGet("api/v1/privileges")]
    public async Task<IActionResult> GetPrivilege([FromHeader(Name = "X-User-Name")] string username)
    {
        var privilege = await GetOrCreatePrivilege(username);

        return Ok(ToDto(privilege));
    }

    [HttpPost("api/v1/privileges")]
    public async Task<IActionResult> BuyTicket([FromHeader(Name = "X-User-Name")] string username, 
        [FromBody] TicketInfoDto ticketInfo)
    {
        var privilege = await GetOrCreatePrivilege(username);

        if (ticketInfo.PaidFromBalance is true)
        {
            var paidByBonuses = Math.Min(ticketInfo.Price, privilege.Balance ?? 0);
            var paidByMoney = ticketInfo.Price - paidByBonuses;
            var newBalance = (privilege.Balance ?? 0) - paidByBonuses;

            await AddHistory(privilege.Id, ticketInfo.TicketUid, ticketInfo.Date, paidByBonuses, "DEBIT_THE_ACCOUNT");
            await UpdateBalance(privilege.Id, newBalance);

            return Ok(new PurchaseInfoDto(paidByBonuses, paidByMoney));
        }
        else
        {
            var paidByBonuses = 0;
            var paidByMoney = ticketInfo.Price;
            var diff = (int)(paidByMoney * 0.1);
            var newBalance = (privilege.Balance ?? 0) + diff;

            await AddHistory(privilege.Id, ticketInfo.TicketUid, ticketInfo.Date, diff, "FILL_IN_BALANCE");
            await UpdateBalance(privilege.Id, newBalance);

            return Ok(new PurchaseInfoDto(paidByBonuses, paidByMoney));
        }
    }

    [HttpDelete("api/v1/privileges/{ticketUid}")]
    public async Task<IActionResult> CancelPurchase([FromRoute] Guid ticketUid)
    {
        var privilegeHistory = await _context.PrivilegeHistories
            .Include(h => h.Privilege)
            .FirstOrDefaultAsync(h => h.TicketUid == ticketUid);

        if (privilegeHistory is null)
            return NotFound($"Ticket with uuid = {ticketUid} wasn't found");

        var newBalance = privilegeHistory.OperationType == "DEBIT_THE_ACCOUNT" 
            ? (privilegeHistory.Privilege!.Balance ?? 0) + privilegeHistory.BalanceDiff
            : (privilegeHistory.Privilege!.Balance ?? 0) - privilegeHistory.BalanceDiff;

        await UpdateBalance(privilegeHistory.PrivilegeId!.Value, newBalance);

        _context.PrivilegeHistories.Remove(privilegeHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Privilege> GetOrCreatePrivilege(string username)
    {
        var privilege = await _context.Privileges
            .Include(p => p.PrivilegeHistories)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);

        if (privilege is null)
        {
            var newPrivilege = new Privilege
            {
                Username = username,
                Status = "BRONZE",
                Balance = 0
            };

            var result = await _context.Privileges.AddAsync(newPrivilege);
            await _context.SaveChangesAsync();

            privilege = result.Entity;
        }

        return privilege;
    }

    private async Task<Privilege> UpdateBalance(int id, int balance)
    {
        var privilege = await _context.Privileges
            .FirstOrDefaultAsync(p => p.Id == id);

        privilege.Balance = balance;
        await _context.SaveChangesAsync();

        return privilege;
    }

    private async Task<PrivilegeHistory> AddHistory(int privilegeId, Guid ticketUid, DateTime dateTime, int balanceDiff, string operationType)
    {
        var history = new PrivilegeHistory
        {
            PrivilegeId = privilegeId,
            TicketUid = ticketUid,
            Datetime = dateTime,
            BalanceDiff = balanceDiff,
            OperationType = operationType
        };

        var result = await _context.PrivilegeHistories.AddAsync(history);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    private static PrivilegeHistoryDto ToDto(PrivilegeHistory history)
    {
        return new PrivilegeHistoryDto(date: history.Datetime,
            ticketUid: history.TicketUid,
            balanceDiff: history.BalanceDiff,
            operationType: history.OperationType);
    }

    private static PrivilegeDto ToDto(Privilege privilege)
    {
        return new PrivilegeDto(balance: privilege.Balance ?? 0,
            status: privilege.Status,
            history: privilege.PrivilegeHistories.Select(h => ToDto(h)).ToList() ?? []);
    }
}
