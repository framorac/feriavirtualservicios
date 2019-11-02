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
        public string tipoVenta { get; set; }

        public Ventas(int id, string username, DateTime fecha, string tipoVenta)
        {
            this.id = id;
            this.username = username;
            this.fecha = fecha;
            this.tipoVenta = tipoVenta;
        }
    }

    [DataContract]
    public class HistoricoEstadoVentas {
        public int Id { get; set; }
        public string TipoEstado { get; set; }
        public int Id_venta { get; set; }
        public DateTime Fecha { get; set; }

        public HistoricoEstadoVentas(int id, string tipoEstado, int id_venta, DateTime fecha)
        {
            Id = id;
            TipoEstado = tipoEstado;
            Id_venta = id_venta;
            Fecha = fecha;
        }
    }
}
