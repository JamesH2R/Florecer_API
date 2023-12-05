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

        [HttpGet]
        [Route("api/AppointmentsByUserId/{userId}")]
        public IHttpActionResult GetAppointmentsByUserId(long userId)
        {
            using (var bd = new FlorecerAppEntities())
            {
                try
                {
                    var appointments = bd.Appointments.Where(a => a.UserId == userId).ToList();

                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest("Error al obtener las citas por userId.");
                }
            }
        }

        [HttpPost]
        [Route("api/CreateAppointment")]
        public IHttpActionResult CreateAppointment(AppointmentsEnt newAppointment)
        {
            using (var bd = new FlorecerAppEntities())
            {
                try
                {
                    var appointmentToAdd = new Appointments
                    {
                        AppointmentId = newAppointment.AppointmentId,
                        Date = newAppointment.Date,
                        Hour = newAppointment.Hour,
                        Available = true          
                    };

                    bd.Appointments.Add(appointmentToAdd);
                    bd.SaveChanges();

                    return Ok("Cita creada exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest("Error al crear la cita.");
                }
            }
        }


        [HttpPost]
        [Route("api/SetAppointment/{appointmentId}")]
        public IHttpActionResult SetAppointment(int appointmentId, int userId)
        {
            using (var bd = new FlorecerAppEntities())
            {
                try
                {
                    var appointment = bd.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);

                    if (appointment != null)
                    {
                        if (appointment.Available)
                        {
                            appointment.Available = false;
                            appointment.UserId = userId;

                            bd.SaveChanges();

                            return Ok("Cita reservada exitosamente.");
                        }
                        else
                        {
                            return BadRequest("La cita ya está reservada.");
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [HttpDelete]
        [Route("api/DeleteAppointment/{appointmentId}")]
        public IHttpActionResult DeleteAppointment(long appointmentId)
        {
            using (var bd = new FlorecerAppEntities())
            {
                try
                {
                    var appointmentToDelete = bd.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);

                    if (appointmentToDelete != null)
                    {
                        bd.Appointments.Remove(appointmentToDelete);
                        bd.SaveChanges();
                        return Ok("Cita eliminada exitosamente.");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return BadRequest("Error al eliminar la cita.");
                }
            }
        }


    }
}
