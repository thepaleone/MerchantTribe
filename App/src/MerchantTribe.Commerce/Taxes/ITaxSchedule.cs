﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MerchantTribe.Web.Geography;

namespace MerchantTribe.Commerce.Taxes
{
    public interface ITaxSchedule
    {
        long TaxScheduleId();
        string TaxScheduleName();
    }
}
