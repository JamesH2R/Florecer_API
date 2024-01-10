using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class AppointmentsTypesEnt
    {
        public long IdAppointmentType { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

    }
}