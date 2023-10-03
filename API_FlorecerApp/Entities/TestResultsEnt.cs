using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class TestResultsEnt
    {
        public long ResultId { get; set; }
        public long PatientId { get; set; }
        public long PersonnelId { get; set; }
        public long TestId { get; set; }
        public DateTime Date { get; set; }
        public string Score { get; set; }
    }
}