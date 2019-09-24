using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    public class Usuario
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Perfil { get; set; }

        public Usuario(string username, string password, string perfil)
        {
            Username = username;
            Password = password;
            Perfil = perfil;
        }
    }
}
