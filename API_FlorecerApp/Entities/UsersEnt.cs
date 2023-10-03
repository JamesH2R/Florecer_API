using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FlorecerApp.Entities
{
    public class UsersEnt
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string NewPassword { get; set; }
        public long RoleId { get; set; }

        public string RoleName { get; set; }

        public string Token { get; set; }

        public bool Status { get; set; }
    }
}