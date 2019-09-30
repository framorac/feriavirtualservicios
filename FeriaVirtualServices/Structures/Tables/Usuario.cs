using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Usuario
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Perfil { get; set; }

        public Usuario(string username, string password, string perfil)
        {
            Username = username;
            Password = password;
            Perfil = perfil;
        }
    }
}
