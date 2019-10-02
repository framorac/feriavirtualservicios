using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Productos
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public string TipoProducto { get; set; }

        public Productos(int id, string nombre, string descripcion, string tipoProducto)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            TipoProducto = tipoProducto;
        }
    }
}
