using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

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

                // Obtener la cadena de conexión y el nombre del contenedor desde la configuración de la aplicación
                string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=florecer;AccountKey=8Vk5WpPH+hsRpM22oiLzG9Zqdg//QJ3RKxN50OCjlJNq1f/26PHWhYq/GZLk+/EYH0KhjDUg5CkH+AStv0rudQ==;EndpointSuffix=core.windows.net";
                string containerName = "florecerapp";

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                // Crea el contenedor si no existe
                container.CreateIfNotExists();

                // Sube el archivo al contenedor
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
                using (var fileStream = file.InputStream)
                {
                    blockBlob.UploadFromStream(fileStream);
                }

                // Guarda la información en la base de datos
                using (var context = new FlorecerAppEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.UserId == test.UserId);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    context.MedicalTests.Add(new MedicalTests
                    {
                        UserId = test.UserId,
                        FileName = file.FileName,
                        FilePath = blockBlob.Uri.ToString(),  // Usa la URI del blob como la ubicación del archivo
                        Date = DateTime.Now
                    });

                    context.SaveChanges();
                }

                return Ok("Evaluación asignada con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /*
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

                        // Obtener el nombre original del archivo
                        string originalFileName = file.FileName;

                        // Guardar el archivo con su nombre original en lugar de cambiarlo
                        string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), originalFileName);
                        file.SaveAs(filePath);

                        context.MedicalTests.Add(new MedicalTests
                        {
                            UserId = test.UserId,
                            FileName = originalFileName,
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
         */


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
                                LastName = u.Name + " " + u.LastName
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
                        LastName = user.LastName,
                        Name = user.Name
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

        [HttpGet]
        [Route("api/GetAllFileNames")]
        public IHttpActionResult GetAllFileNames()
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener los resultados de la base de datos
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var testResults = context.TestResults
                        .Where(tr => allowedExtensions.Any(ext => tr.FilePath.EndsWith(ext)))
                        .ToList();

                    // Convertir los resultados a objetos TestResultsEnt
                    var results = testResults.Select(tr => new TestResultsEnt
                    {
                        ResultId = tr.ResultId, // Ajustar las propiedades según tu modelo TestResultsEnt
                        FilePath = Path.GetFileName(tr.FilePath), // Obtener solo el nombre del archivo
                                                                  // Agregar otras propiedades según sea necesario
                    }).ToList();

                    return Ok(results); // Devolver la lista de objetos TestResultsEnt
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recuperar los nombres de archivo: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/DownloadTestResult/{ResultId}")]
        public IHttpActionResult DownloadTestResult(long ResultId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener la evaluación del usuario
                    var evaluation = context.TestResults.FirstOrDefault(mt => mt.ResultId == ResultId);

                    if (evaluation == null)
                    {
                        return NotFound(); // No hay evaluación asignada para este usuario
                    }

                    // Obtener el nombre del archivo de la ruta completa
                    var fileName = Path.GetFileName(evaluation.FilePath);

                    // Leer el contenido del archivo
                    var fileBytes = File.ReadAllBytes(evaluation.FilePath);

                    // Crear una respuesta HTTP con el contenido del archivo
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };

                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    // Devolver la respuesta HTTP al cliente
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


        [HttpDelete]
        [Route("api/DeleteTestResult/{ResultId}")]
        public IHttpActionResult DeleteTestResult(long ResultId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Obtener evaluaciones del usuario
                    var evaluations = context.TestResults
                        .Where(mt => mt.ResultId == ResultId)
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
                        context.TestResults.Remove(evaluation);
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
        [Route("api/GetUserEvaluations/{userId}")]
        public IHttpActionResult GetUserEvaluations(long userId)
        {
            try
            {

                using (var context = new FlorecerAppEntities())
                {
                    // Obtener evaluaciones del usuario
                    var evaluations = context.MedicalTests
                        .Where(mt => mt.UserId == userId)
                        .ToList();

                    // Mapear evaluaciones a una lista de objetos anónimos para el frontend
                    var evaluationList = evaluations.Select(e => new
                    {
                        TestId = e.TestId,
                        FileName = e.FileName
                    }).ToList();

                    return Ok(evaluationList);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error inesperado: {ex.Message}");
            }
        }

        
        [HttpPost]
        [Route("api/DownloadEvaluation/{userId}/{selectedTestId}")]
        public IHttpActionResult DownloadEvaluation(long userId, long selectedTestId)
        {
            try
            {
                using (var context = new FlorecerAppEntities())
                {
                    // Verificar si la evaluación pertenece al usuario
                    var evaluation = context.MedicalTests.FirstOrDefault(mt => mt.UserId == userId && mt.TestId == selectedTestId);

                    if (evaluation == null)
                    {
                        // La evaluación no pertenece al usuario
                        return Content(HttpStatusCode.NotFound, "La evaluación no existe o no pertenece al usuario.");
                    }

                    // Obtener el nombre original del archivo
                    string originalFileName = evaluation.FileName;

                    // Obtener el path del archivo desde la entidad MedicalTests
                    string filePath = evaluation.FilePath;

                    // Verificar si el archivo existe en el sistema de archivos
                    if (!File.Exists(filePath))
                    {
                        return Content(HttpStatusCode.NotFound, "El archivo no se encuentra en el sistema de archivos.");
                    }

                    // Leer los bytes del archivo
                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    // Devolver el archivo como respuesta
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };

                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = originalFileName
                    };

                    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    return ResponseMessage(result);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Manejar la excepción de acceso no autorizado
                return Content(HttpStatusCode.Unauthorized, $"Error al acceder al archivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
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



        /*
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

        */

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
