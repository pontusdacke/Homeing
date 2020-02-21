using System;
using System.Collections.Generic;

namespace HomingProgram
{
    public class AppartmentHistory
    {
        public List<AppartmentStatistics> AppartmentStatistics { get; set; } = new List<AppartmentStatistics>();

        public bool IsShownToday(string objectNumber)
        {
            return AppartmentStatistics.Exists(x => x.ObjectNumber == objectNumber && x.DatesShown.Contains(DateTime.UtcNow.Date));
        }
    }
}
