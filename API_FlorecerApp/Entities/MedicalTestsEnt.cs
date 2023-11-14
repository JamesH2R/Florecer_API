using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class MedicalTestsEnt
    {
        public long TestId { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Date { get; set; }
    }
}