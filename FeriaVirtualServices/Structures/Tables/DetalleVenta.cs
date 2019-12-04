﻿using System;
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

        public DetalleVenta(int id, string nombre_producto, int id_oferta, int cantidad)
        {
            Id = id;
            Nombre_producto = nombre_producto;
            Id_oferta = id_oferta;
            Cantidad = cantidad;
        }
    }
}
