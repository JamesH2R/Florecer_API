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
    
    public partial class Binnacle
    {
        public long BinnacleId { get; set; }
        public Nullable<long> UserId { get; set; }
        public string PerformedAction { get; set; }
        public Nullable<System.DateTime> OccurrencyDate { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
