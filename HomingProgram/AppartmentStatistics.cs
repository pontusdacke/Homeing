using System;
using System.Collections.Generic;

namespace HomingProgram
{
    public class AppartmentStatistics
    {
        public string ObjectNumber { get; set; }
        public List<DateTime> DatesShown { get; set; } = new List<DateTime>();
        public int Count => DatesShown.Count;
    }
}
