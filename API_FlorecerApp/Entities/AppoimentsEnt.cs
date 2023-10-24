using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class AppoimentsEnt
    {
        public long AppoimentId { get; set; }
        public long PatientId { get; set; }
        public DateTime Date { get; set; }
        public string Hour { get; set; }
        public string Notes { get; set; }
    }
}