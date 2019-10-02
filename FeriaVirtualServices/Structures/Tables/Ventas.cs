using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Ventas
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public DateTime fecha { get; set; }
        [DataMember]
        public string tipoEstado { get; set; }
        [DataMember]
        public string tipoVenta { get; set; }

        public Ventas(int id, string username, DateTime fecha, string tipoEstado, string tipoVenta)
        {
            this.id = id;
            this.username = username;
            this.fecha = fecha;
            this.tipoEstado = tipoEstado;
            this.tipoVenta = tipoVenta;
        }
    }
}
