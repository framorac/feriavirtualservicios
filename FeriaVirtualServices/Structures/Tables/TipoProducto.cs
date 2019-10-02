using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class TipoProducto
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string tipo { get; set; }
        [DataMember]
        public string descripcion { get; set; }

        public TipoProducto(int id, string tipo, string descripcion)
        {
            this.id = id;
            this.tipo = tipo;
            this.descripcion = descripcion;
        }
    }
}
