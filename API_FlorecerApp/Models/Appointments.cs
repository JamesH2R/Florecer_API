//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API_FlorecerApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Appointments
    {
        public long AppointmentId { get; set; }
        public System.DateTime Date { get; set; }
        public string Hour { get; set; }
        public bool Available { get; set; }
        public Nullable<long> UserId { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
