using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class BinnacleEnt
    {
        public long BinnacleId { get; set; }
        public long UserId { get; set; }
        public string PerformedAction { get; set; }
        public DateTime OccurrencyDate { get; set; }
    }
}