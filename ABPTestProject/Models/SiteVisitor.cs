﻿using System;
using System.Collections.Generic;

namespace ABPTestProject.Models;

public partial class SiteVisitor
{
    public string DeviceToken { get; set; } = null!;

    public string? ButtonColor { get; set; }

    public decimal? Price { get; set; }
}
