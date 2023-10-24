using System;
using System.Linq;
using System.Web.Http;
using API_FlorecerApp.Entities;
using API_FlorecerApp.Models;
using API_FlorecerApp.App_Start;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;

namespace API_FlorecerApp.Controllers
{

    public class UserAdminController : ApiController
    {

        [HttpGet]
        [Route("api/UserConsultation")]
        public List<UsersEnt> UserConsultation()
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Users
                             select x).ToList();

                if (datos.Count > 0)
                {
                    var resp = new List<UsersEnt>();
                    foreach (var item in datos)
                    {
                        resp.Add(new UsersEnt
                        {
                            UserId = item.UserId,
                            Name = item.Name,
                            LastName = item.LastName,
                            Email = item.Email,
                            Status = item.Status,
                            RoleId = item.RoleId
                        });
                    }
                    return resp;
                }
                else
                {
                    return new List<UsersEnt>();
                }
            }
        }

        [HttpPut]
        [Route("api/InactivateUser/{userId}")]
        public IHttpActionResult InactivateUser(int userId)
        {
            using (var context = new FlorecerAppEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.Status = false; 
                context.SaveChanges();
            }

            return Ok("Usuario inactivado con éxito");
        }

        [HttpGet]
        [Route("api/SearchUsers")]
        public List<UsersEnt> SearchUsers(string searchTerm)
        {
            using (var context = new FlorecerAppEntities())
            {
                var searchResults = context.Users
                    .Where(u => u.Name.Contains(searchTerm))
                    .Select(u => new UsersEnt
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        LastName = u.LastName,
                        Email = u.Email,
                        Status = u.Status,
                        RoleId = u.RoleId
                    })
                    .ToList();

                return searchResults;
            }
        }

    }
} 

