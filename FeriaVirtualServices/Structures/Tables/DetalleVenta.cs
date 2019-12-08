using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class DetalleVenta
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nombre_producto { get; set; }
        [DataMember]
        public int Id_oferta { get; set; }
        [DataMember]
        public int Cantidad { get; set; }
        [DataMember]
        public int IdProducto { get; set; }

        public DetalleVenta(int id, string nombre_producto, int id_oferta, int cantidad, int idProducto = 0)
        {
            Id = id;
            Nombre_producto = nombre_producto;
            Id_oferta = id_oferta;
            Cantidad = cantidad;
            IdProducto = idProducto;
        }

    }
    [DataContract]
    public class DetalleVentaDB
    {
        [DataMember]
        public int IdProducto { get; set; }
        [DataMember]
        public int IdVenta { get; set; }
        [DataMember]
        public int Cantidad { get; set; }

        public DetalleVentaDB(int idProducto, int idVenta, int cantidad)
        {
            IdProducto = idProducto;
            IdVenta = idVenta;
            Cantidad = cantidad;
        }
    }
}
