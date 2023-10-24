using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_FlorecerApp.Entities;
using API_FlorecerApp.Models;

namespace API_FlorecerApp.Controllers
{
    public class AppointmentController : ApiController
    {

        [HttpPost]
        [Route("api/NewAppointment")]
        [AllowAnonymous]
        public IHttpActionResult NewAppointment(AppoimentsEnt entidad)
        {
            using (var bd = new FlorecerAppEntities())
            {
                if (entidad == null)
                {
                    return BadRequest("La cita es nula");
                }

                try
                {

                    Appointments newAppointment = new Appointments
                    {
                        AppointmentId = entidad.AppoimentId,
                        PatientId = entidad.PatientId,
                        Date = entidad.Date,
                        Hour = entidad.Hour,
                        Notes = entidad.Notes
                    };

                    bd.Appointments.Add(newAppointment);
                    bd.SaveChanges();
                    return Ok("Cita creada con éxito");
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

    }
}
