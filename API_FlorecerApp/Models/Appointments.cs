//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API_FlorecerApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Appointments
    {
        public long AppointmentId { get; set; }
        public long PatientId { get; set; }
        public System.DateTime Date { get; set; }
        public string Hour { get; set; }
        public string Notes { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
