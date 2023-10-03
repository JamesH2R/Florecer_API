using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class MedicalPersonnelEnt
    {
        public long PersonnelId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public long SpecialtyId { get; set; }
    }
}