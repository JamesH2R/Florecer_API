using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class AppointmentsEnt
    {
        public long AppointmentId { get; set; }
        public System.DateTime Date { get; set; }
        public string Hour { get; set; }
        public bool Available { get; set; }
        public Nullable<long> UserId { get; set; }

    }
}