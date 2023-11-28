using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using API_FlorecerApp.Entities;
using API_FlorecerApp.Models;
using Newtonsoft.Json;

namespace API_FlorecerApp.Controllers
{
    public class TestController : ApiController
    {
        //Métodos Rol ADMIN

        [HttpPost]
        [Route("api/AssignEvaluation")]
        public IHttpActionResult AssignEvaluation()
        {
            try
            {
                var testJson = HttpContext.Current.Request.Form["test"];
                var file = HttpContext.Current.Request.Files["file"];
                MedicalTestsEnt test = JsonConvert.DeserializeObject<MedicalTestsEnt>(testJson);

                using (var context = new FlorecerAppEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.UserId == test.UserId);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    string fileName = Path.GetFileName(test.FileName);
                    string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
                    file.SaveAs(filePath);

                    context.MedicalTests.Add(new MedicalTests
                    {
                        UserId = test.UserId,
                        FileName = fileName,
                        FilePath = filePath,
                        Date = DateTime.Now
                    });

                    context.SaveChanges();

                    return Ok("Evaluación asignada con éxito.");

                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/getUserDropdown")]
        public IHttpActionResult GetUserDropdown()
        {
            using (FlorecerAppEntities fa = new FlorecerAppEntities())
            {
                var query = from u in fa.Users
                            select new UsersEnt
                            {
                                UserId = u.UserId,
                                LastName = u.LastName
                            };

                IList<UsersEnt> result = query.ToList();
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("api/getUserById/{userId}")]
        public IHttpActionResult GetUserById(long userId)
        {
            using (FlorecerAppEntities fa = new FlorecerAppEntities())
            {
                var user = fa.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    var result = new UsersEnt
                    {
                        UserId = user.UserId,
                        LastName = user.LastName
                    };
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpDelete]
        [Route("api/DeleteEvaluations/{userId}")]
        public IHttpActionResult DeleteEvaluations(long userId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener evaluaciones del usuario
                    var evaluations = context.MedicalTests
                        .Where(mt => mt.UserId == userId)
                        .ToList();

                    if (evaluations.Count == 0)
                    {
                        return NotFound(); // No hay evaluaciones asignadas para este usuario
                    }

                    // Eliminar archivos y entradas de la base de datos
                    foreach (var evaluation in evaluations)
                    {
                        // Eliminar archivo del directorio App_Data
                        if (File.Exists(evaluation.FilePath))
                        {
                            File.Delete(evaluation.FilePath);
                        }

                        // Eliminar entrada de la base de datos
                        context.MedicalTests.Remove(evaluation);
                    }

                    context.SaveChanges();

                    return Ok("Evaluaciones eliminados con éxito.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar evaluaciones: {ex.Message}");
            }
        }


        //Métodos Rol USUARIO

        [HttpGet]
        [Route("api/DownloadEvaluation/{userId}")]
        public IHttpActionResult DownloadEvaluation(long userId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener evaluaciones del usuario
                    var evaluations = context.MedicalTests
                        .Where(mt => mt.UserId == userId)
                        .ToList();

                    if (evaluations.Count == 0)
                    {
                        return NotFound(); // No hay evaluaciones asignadas para este usuario
                    }

                    // Crear un archivo ZIP para contener los archivos de evaluación
                    var zipFileName = $"Evaluaciones_{userId}.zip";
                    var zipFilePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), zipFileName);

                    // Utilizar System.IO.Compression.ZipFile para crear el archivo ZIP
                    using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var evaluation in evaluations)
                        {
                            // Agregar cada archivo de evaluación al archivo ZIP
                            var entry = archive.CreateEntry(evaluation.FileName);
                            using (var entryStream = entry.Open())
                            using (var fileStream = File.OpenRead(evaluation.FilePath))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    // Devolver el archivo ZIP al cliente
                    var fileBytes = File.ReadAllBytes(zipFilePath);
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = zipFileName
                    };

                    // Después de Devolver el Archivo ZIP al Cliente
                    //File.Delete(zipFilePath);

                    return ResponseMessage(response);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error al acceder al archivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error inesperado: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("api/SendResult")]
        public IHttpActionResult SendResult()
        {
            try
            {
                // Recupera el archivo
                var file = HttpContext.Current.Request.Files["file"];

                // Verifica si se proporcionó un archivo
                if (file == null || file.ContentLength == 0)
                {
                    return BadRequest("No se proporcionó ningún archivo en la solicitud.");
                }

                // Lógica para guardar el archivo y los datos en la base de datos
                using (var context = new FlorecerAppEntities())
                {
                    context.TestResults.Add(new TestResults
                    {
                        RoleId = 1, // RoleId del administrador
                        FilePath = SaveFileAndGetPath(file),
                        Date = DateTime.Now
                    });

                    context.SaveChanges();
                }

                return Ok("Resultado recibido con éxito. Se ha enviado al administrador.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al enviar el resultado: {ex.Message}");
            }
        }
        private string SaveFileAndGetPath(HttpPostedFile file)
        {
            // Guarda el archivo en ~/App_Data
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
            file.SaveAs(filePath);

            return filePath;
        }

        [HttpGet]
        [Route("api/GetUserEvaluationNames/{userId}")]
        public IHttpActionResult GetUserEvaluationNames(long userId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener evaluaciones del usuario
                    var evaluations = context.MedicalTests
                        .Where(mt => mt.UserId == userId)
                        .ToList();

                    if (evaluations.Count == 0)
                    {
                        return NotFound(); // No hay evaluaciones asignadas para este usuario
                    }

                    // Crear una lista para almacenar los nombres de archivo
                    List<string> fileNames = new List<string>();

                    foreach (var evaluation in evaluations)
                    {
                        fileNames.Add(evaluation.FileName);
                    }

                    return Ok(fileNames);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error inesperado: {ex.Message}");
            }
        }


    }
}
