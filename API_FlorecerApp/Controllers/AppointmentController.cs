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

        [HttpGet]
        [Route("api/Appointments")]
        public IHttpActionResult Appointments()
        {

            using (var bd = new FlorecerAppEntities())
            {
                using (var transaction = bd.Database.BeginTransaction())
                {
                    try
                    {
                        FreePastAppointments(bd);
                        var appointments = bd.Appointments.ToList();
                        transaction.Commit();
                        return Ok(appointments);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(ex);
                    }
                }
            }
        }

        //[HttpPost]
        //[Route("api/SetAppointment/{appointmentId}")]
        //public IHttpActionResult SetAppointment(int appointmentId)
        //{

        //    using (var bd = new FlorecerAppEntities())
        //    {
        //        try
        //        {

        //            var appointment = bd.Appointments.Find(appointmentId);

        //            if (appointment != null)
        //            {
        //                // Marcar la cita como reservada (ajusta según tu modelo)
        //                appointment.Available = true;

        //                bd.Appointments.Add(new AppoimentsEnt
        //                {
        //                    UserId = ,
        //                    AppointmentId = appointmentId
        //                });

        //                // Guardar los cambios en la base de datos
        //                bd.SaveChanges();

        //                return Ok("Cita reservada exitosamente.");
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return InternalServerError(ex);
        //        }
        //    }
        //}


        [HttpPost]
        private IHttpActionResult FreePastAppointments(FlorecerAppEntities bd)
        {
            try
            {

                    var actualDate = DateTime.UtcNow;

                    var pastAppointments = bd.Appointments.Where(c => c.Date < actualDate && !c.Available);

                    foreach (var schedule in pastAppointments)
                    {
                        schedule.Available = true;
                    }

                    bd.SaveChanges();

                    return Ok();
                
                    
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
