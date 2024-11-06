using System.Text.Json.Serialization;

namespace BonusService.Dto;

public class PurchaseInfoDto(int paidByBonuses, int paidByMoney)
{
    [JsonPropertyName("paidByBonuses")]
    public int PaidByBonuses { get; set; } = paidByBonuses;

    [JsonPropertyName("paidByMoney")]
    public int PaidByMoney { get; set; } = paidByMoney;
}
