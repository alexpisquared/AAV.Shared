﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Db.MinFinInv.PowerTools.Models;

public partial class VAcntHistByAcnt
{
    public string AcntNumber { get; set; }

    public string InvAccountId { get; set; }

    public DateOnly Date { get; set; }

    public decimal? Txn { get; set; }

    public decimal? TotalInvested { get; set; }

    public decimal? Balance { get; set; }

    public decimal? UnitValue { get; set; }

    public decimal? BalanceInUnits { get; set; }

    public string Notes { get; set; }
}