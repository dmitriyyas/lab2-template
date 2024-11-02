using System;
using System.Collections.Generic;

namespace BonusService.Models;

public partial class Privilege
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Balance { get; set; }

    public virtual ICollection<PrivilegeHistory> PrivilegeHistories { get; set; } = new List<PrivilegeHistory>();
}
