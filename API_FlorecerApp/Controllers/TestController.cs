using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using API_FlorecerApp.Entities;
using API_FlorecerApp.Models;
using Newtonsoft.Json;

namespace API_FlorecerApp.Controllers
{
    public class TestController : ApiController
    {
        //Método Rol Admin

        //[HttpPost]
        //[Route("api/AssignEvaluation")]
        //public IHttpActionResult AssignEvaluation()
        //{
        //    try
        //    {
        //        var testJson = HttpContext.Current.Request.Form["test"];
        //        var file = HttpContext.Current.Request.Files["file"];
        //        MedicalTestsEnt test = JsonConvert.DeserializeObject<MedicalTestsEnt>(testJson);

        //        using (var context = new FlorecerAppEntities())
        //        {
        //            var user = context.Users.FirstOrDefault(u => u.UserId == test.UserId);

        //            if (user == null)
        //            {
        //                return NotFound();
        //            }

        //            string fileName = Path.GetFileName(test.FileName);
        //            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
        //            file.SaveAs(filePath);

        //            context.MedicalTests.Add(new MedicalTest
        //            {
        //                UserId = test.UserId,
        //                FileName = fileName,
        //                FilePath = filePath,
        //                Date = test.Date
        //            });

        //            context.SaveChanges();

        //            return Ok("Evaluación asignada con éxito.");

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.Message);
        //    }

        //}



    }
}
