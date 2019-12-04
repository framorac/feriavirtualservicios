using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{

    [DataContract]
    public class DetalleOferta
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Id_oferta { get; set; }

        [DataMember]
        public string Nombre_producto { get; set; }

        [DataMember]
        public int Cantidad { get; set; }

        [DataMember]
        public int Precio { get; set; }

        public DetalleOferta(int id, int id_oferta, string nombre_producto, int cantidad, int precio)
        {
            Id = id;
            Id_oferta = id_oferta;
            Nombre_producto = nombre_producto;
            Cantidad = cantidad;
            Precio = precio;
        }
    }
}
