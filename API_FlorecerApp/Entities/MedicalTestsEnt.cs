using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class MedicalTestsEnt
    {
        public long TestId { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public string TestType { get; set; }
    }
}