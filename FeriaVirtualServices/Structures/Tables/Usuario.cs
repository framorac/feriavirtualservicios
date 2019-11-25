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
        public int id { get; set; } 
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Perfil { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Apellido { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime FechaCreacion { get; set; }



        //public Usuario(string username, string password, string perfil)
        //{
        //    Username = username;
        //    Password = password;
        //    Perfil = perfil;
        //}
        public Usuario() { }
        public Usuario(int id, string username, string password, string perfil, string nombre, string apellido, string email, DateTime fechaCreacion)
        {
            this.id = id;
            Username = username;
            Password = password;
            Perfil = perfil;
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            FechaCreacion = fechaCreacion;
        }
    }
}
