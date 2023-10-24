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

    public class UserController : ApiController
    {


        //Token Generator
        TokenGenerator tok = new TokenGenerator();


        [HttpPost]
        [Route("api/Login")]
        [AllowAnonymous]
        public IHttpActionResult Login(LoginEnt entidad)
        {
            TokenGenerator tok = new TokenGenerator();

            using (var bd = new FlorecerAppEntities())
            {

                var user = bd.Users.SingleOrDefault(x => x.Email == entidad.Email && x.Status == true);

                if (user != null)
                {

                    if (BCrypt.Net.BCrypt.Verify(entidad.Password, user.Password))
                    {
                        if (user.TemporalKey.Value && user.Expiration < DateTime.Now)
                        {
                            return null;
                        }

                        UsersEnt resp = new UsersEnt();
                        resp.Email = user.Email;
                        resp.Name = user.Name;
                        resp.LastName = user.LastName;
                        resp.Status = user.Status;
                        resp.RoleId = user.RoleId;
                        resp.RoleName = user.Roles.RoleName;
                        resp.UserId = user.UserId;
                        resp.Token = tok.GenerateTokenJwt(user.UserId);
                        return Ok(resp);
                    }
                }
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/Register")]
        [AllowAnonymous]
        public IHttpActionResult Register(UsersEnt entidad)
        {
            using (var bd = new FlorecerAppEntities())
            {
                var existingUser = bd.Users.FirstOrDefault(x => x.Email == entidad.Email);

                if (existingUser != null)
                {
                    return BadRequest("El correo ya está registrado.");
                }

                Users table = new Users();
                table.Email = entidad.Email;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(entidad.Password);
                table.Password = hashedPassword;
                table.LastName = entidad.LastName;
                table.Name = entidad.Name;
                table.Status = true;
                table.TemporalKey = false;
                table.Expiration = DateTime.Now;
                table.RoleId = 2;

                bd.Users.Add(table);
                bd.SaveChanges();

                return Ok("Usuario registrado correctamente.");
            }
        }

        [HttpPost]
        [Route("api/RecoverKey")]
        [AllowAnonymous]
        public bool RecoverKey(UsersEnt entidad)
        {
            Utilities util = new Utilities();

            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Users
                             where x.Email == entidad.Email
                                           && x.Status == true
                             select x).FirstOrDefault();

                if (datos != null)
                {
                    string pass = util.CreatePassword();
                    string mensaje = "Estimado(a): " + datos.Name + ". Se ha generado la siguiente contraseña temporal: " + pass + " valida por 15 minutos";
                    util.SendEmail(datos.Email, "Recuperar Contraseña", mensaje);

                    //Update LinQ
                    datos.Password = pass;
                    datos.TemporalKey = true;
                    datos.Expiration = DateTime.Now.AddMinutes(15);
                    bd.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        [HttpPut]
        [Route("api/ChangePassword")]
        public int ChangePassword(UsersEnt entidad)
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Users
                             where x.UserId == entidad.UserId
                             select x).FirstOrDefault();

                if (datos != null)
                {
                    datos.Password = entidad.NewPassword;
                    datos.TemporalKey = false;
                    datos.Expiration = DateTime.Now;
                    return bd.SaveChanges();
                }

                return 0;
            }
        }

        [HttpGet]
        [Route("api/ConsultUsers")]
        public List<UsersEnt> ConsultUsuarios()
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
                            Email = item.Email,
                            Name = item.Name,
                            LastName = item.LastName,
                            Status = item.Status,
                            RoleId = item.RoleId,
                            UserId = item.UserId
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

        [HttpGet]
        [Route("api/ConsultUser")]
        public UsersEnt ConsultUser(long q)
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Users
                             where x.UserId == q
                             select x).FirstOrDefault();

                if (datos != null)
                {
                    UsersEnt resp = new UsersEnt();
                    resp.Name = datos.Name;
                    resp.LastName = datos.LastName;
                    resp.Email = datos.Email;
                    resp.Status = datos.Status;
                    resp.RoleId = datos.RoleId;
                    resp.UserId = datos.UserId;
                    return resp;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet]
        [Route("api/ConsultRoles")]
        public List<RolesEnt> ConsultRoles()
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Roles
                             where x.Status == true
                             select x).ToList();

                if (datos.Count > 0)
                {
                    var resp = new List<RolesEnt>();
                    foreach (var item in datos)
                    {
                        resp.Add(new RolesEnt
                        {
                            RoleId = item.RoleId,
                            RoleName = item.RoleName,
                        });
                    }
                    return resp;
                }
                else
                {
                    return new List<RolesEnt>();
                }
            }
        }


        [HttpPut]
        [Route("api/EditUser")]
        public int EditUser(UsersEnt entidad)
        {
            using (var bd = new FlorecerAppEntities())
            {
                var datos = (from x in bd.Users
                             where x.UserId == entidad.UserId
                             select x).FirstOrDefault();

                if (datos != null)
                {
                    datos.Name = entidad.Name;
                    datos.LastName = entidad.LastName;
                    datos.Email = entidad.Email;
                    return bd.SaveChanges();
                }

                return 0;
            }
        }
    }
}

