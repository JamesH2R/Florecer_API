using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web.Http;
using API_FlorecerApp.Entities;
using API_FlorecerApp.Models;
using System.Web.UI.WebControls;


namespace API_FlorecerApp.Controllers
{


    public class AppointmentController : ApiController
    {
        /*
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
        */
        //[HttpGet]
        //[Route("api/ConsultAppointTypes")]
        //public IHttpActionResult ConsultAppointTypes()
        //{
        //    try
        //    {
        //        using (var context = new FlorecerAppEntities())
        //        {
        //            var appointmentTypesDTO = context.AppointmentTypes
        //                .Select(t => new
        //                {
        //                    IdAppointmentType = t.IdAppointmentType,
        //                    Name = t.Name,
        //                    Description = t.Description
        //                })
        //                .ToList();

        //            // Convert the anonymous type to your original entity type
        //            var result = appointmentTypesDTO.Select(dto => new AppointmentTypes
        //            {
        //                IdAppointmentType = dto.IdAppointmentType,
        //                Name = dto.Name,
        //                Description = dto.Description
        //            }).ToList();

        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle errors, for example, return a 500 error with the error message
        //        return InternalServerError(ex);
        //    }
        //}



        [HttpGet]
        [Route("api/ConsultAppointTypes")]
        public IHttpActionResult ConsultAppointTypes()
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    var appointmentTypes = context.AppointmentTypes
                        .Select(t => new System.Web.Mvc.SelectListItem
                        {
                            Value = t.IdAppointmentType.ToString(),
                            Text = t.Name
                        })
                        .ToList();

                    return Ok(appointmentTypes);
                }
            }
            catch (Exception ex)
            {
                // Handle errors, for example, return a 500 error with the error message
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        [Route("api/ConsultReservedAppointments")]
        public IHttpActionResult ConsultReservedAppointments()
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    var reservedAppointments = context.Appointments
                        .Where(a => a.Status) // Filtrar por citas con estado 'Reservada'
                        .Select(a => a.DateTime)
                        .ToList();

                    return Ok(reservedAppointments);
                }
            }
            catch (Exception ex)
            {
                // Manejar errores: por ejemplo, retornar un error 500 con el mensaje de error
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/GetAppointments")]
        public IHttpActionResult GetAppointments()
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    var appointments = context.Appointments
                        .Select(a => new
                        {
                            AppointmentId = a.AppointmentId,
                            DateTime = a.DateTime,
                            Status = a.Status,
                            UserId = a.UserId,
                            AppointmentType = a.AppointmentType
                        })
                        .ToList()
                        .Select(a => new Appointments
                        {
                            AppointmentId = a.AppointmentId,
                            DateTime = a.DateTime,
                            Status = a.Status,
                            UserId = a.UserId,
                            AppointmentType = a.AppointmentType
                        })
                        .ToList();

                    return Ok(appointments);
                }

            }
            catch (Exception ex)
            {
                // Handle errors, for example, return a 500 error with the error message
                return InternalServerError(ex);
            }
        }


        //[HttpPost]
        //[Route("api/RegisterAppointment")]
        //[AllowAnonymous]
        //public IHttpActionResult RegisterAppointment(AppointmentsEnt entidad)
        //{
        //    try
        //    {
        //        using (var bd = new FlorecerAppEntities())
        //        {
        //            var existingAppointment = bd.Appointments.FirstOrDefault(c => c.DateTime == entidad.DateTime && c.UserId == entidad.UserId);

        //            if (existingAppointment != null)
        //            {
        //                return BadRequest("Ya existe una cita registrada para esta fecha y usuario.");
        //            }

        //            var citaRegistrada = new Appointments
        //            {
        //                DateTime = entidad.DateTime,
        //                UserId = entidad.UserId,
        //                AppointmentType = entidad.AppointmentType,
        //                Status = true // Establecer el estado por defecto como 'true'
        //            };

        //            bd.Appointments.Add(citaRegistrada);
        //            bd.SaveChanges();

        //            return Ok("Cita médica registrada correctamente.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}



        [HttpPost]
        [Route("api/RegisterAppointment")]
        [AllowAnonymous]
        public IHttpActionResult RegisterAppointment(AppointmentsEnt entidad)
        {
            try
            {
                using (var bd = new FlorecerAppEntities())
                {
                    var existingAppointment = bd.Appointments.FirstOrDefault(c => c.DateTime == entidad.DateTime && c.UserId == entidad.UserId);

                    if (existingAppointment != null)
                    {
                        return BadRequest("Ya existe una cita registrada para esta fecha y usuario.");
                    }

                    var citaRegistrada = new Appointments
                    {
                        DateTime = entidad.DateTime,
                        UserId = entidad.UserId,
                        AppointmentType = entidad.AppointmentType,
                        Status = true // Establecer el estado por defecto como 'true'
                    };

                    bd.Appointments.Add(citaRegistrada);
                    bd.SaveChanges();

                    // Devolver solo el Id de la cita registrada
                    return Ok(citaRegistrada.AppointmentId);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/AdminAppointments")]
        public List<AppointmentsEnt> AdminAppointments()
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Appointments
                             select x).ToList();

                if (datos.Count > 0)
                {
                    var resp = new List<AppointmentsEnt>();
                    foreach (var item in datos)
                    {
                        resp.Add(new AppointmentsEnt
                        {
                            UserId = item.UserId,
                            AppointmentId = item.AppointmentId,
                            DateTime = item.DateTime,
                            AppointmentType = item.AppointmentType
                        });
                    }
                    return resp;
                }
                else
                {
                    return new List<AppointmentsEnt>();
                }
            }
        }


        [HttpGet]
        [Route("api/UsersAppointments/{userId}")]
        public IHttpActionResult UsersAppointments(long userId)
        {
            try
            {

                using (var context = new FlorecerAppEntities())
                {
                    // Obtener citas del usuario específico
                    var appointments = context.Appointments
                        .Where(a => a.UserId == userId)
                        .Select(a => new
                        {
                            UserId = a.UserId,
                            AppointmentId = a.AppointmentId,
                            DateTime = a.DateTime,
                            AppointmentType = a.AppointmentType
                        })
                        .ToList();

                    return Ok(appointments);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error inesperado: {ex.Message}"));

            }
        }

        /*

           [HttpPut]
           [Route("api/InactivateAppointment/{appointmentId}")]
           public IHttpActionResult InactivateAppointment(int appointmentId)
           {
               try
               {
                   using (var context = new FlorecerAppEntities())
                   {
                       var appointments = context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
                       if (appointments == null)
                       {
                           return NotFound();
                       }

                       appointments.Status = false;
                       context.SaveChanges();
                   }

                   return Ok("Cita cancelada con éxito");
               }
               catch (Exception ex)
               {
                   return BadRequest(ex.Message);
               }
           }

     */


        //Eliminar cita

        [HttpDelete]
        [Route("api/CancelAppointment/{AppointmentId}")]
        public IHttpActionResult CancelAppointment(int AppointmentId)
        {
            using (var bd = new FlorecerAppEntities())
            {
                var appointment = bd.Appointments.FirstOrDefault(a => a.AppointmentId == AppointmentId);
                if (appointment != null)
                {
                    bd.Appointments.Remove(appointment);
                    bd.SaveChanges();
                    return Ok("Cita eliminada exitosamente");
                }
                else
                {
                    return NotFound();
                }

            }
        }

    }

}

