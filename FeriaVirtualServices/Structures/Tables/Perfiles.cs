using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Perfiles
    {
        public int Id_perfil { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }

        public Perfiles(int id_perfil, string tipo, string descripcion)
        {
            Id_perfil = id_perfil;
            Tipo = tipo;
            Descripcion = descripcion;
        }
    }
}
