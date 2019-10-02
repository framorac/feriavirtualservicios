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
        [DataMember]
        public int Id_perfil { get; set; }
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Descripcion { get; set; }

        public Perfiles(int id_perfil, string tipo, string descripcion)
        {
            Id_perfil = id_perfil;
            Tipo = tipo;
            Descripcion = descripcion;
        }
    }
}
