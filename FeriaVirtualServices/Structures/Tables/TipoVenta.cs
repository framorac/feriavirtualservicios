﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class TipoVenta
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Descripcion { get; set; }

        public TipoVenta(int id, string tipo, string descripcion)
        {
            this.Id = id;
            this.Tipo = tipo;
            this.Descripcion = descripcion;
        }
    }
}
