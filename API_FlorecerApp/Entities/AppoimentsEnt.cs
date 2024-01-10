using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class AppointmentsEnt
    {
        public long AppointmentId { get; set; }

        public DateTime DateTime { get; set; }

        public long AppointmentType { get; set; }

        public long UserId { get; set; }

        public bool Status { get; set; }

    }
}