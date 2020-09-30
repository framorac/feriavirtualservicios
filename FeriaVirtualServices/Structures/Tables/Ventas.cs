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
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string TipoEstado { get; set; }
        [DataMember]
        public int Id_venta { get; set; }
        [DataMember]
        public DateTime Fecha { get; set; }
        [DataMember]
        public bool Activo { get; set; }

        [DataMember]
        public bool Islocal { get; set; }

        public HistoricoEstadoVentas(int id, string tipoEstado, int id_venta, DateTime fecha, bool activo, bool isLocal)
        {
            Id = id;
            TipoEstado = tipoEstado;
            Id_venta = id_venta;
            Fecha = fecha;
            Activo = activo;
            Islocal = isLocal;
        }
    }

    [DataContract]
    public class GenericString
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Generic { get; set; }

        public GenericString(int id, string generic)
        {
            Id = id;
            Generic = generic;
        }
    }

    [DataContract]
    public class VentaCompleta {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string NombreApellido { get; set; }
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public string Estado{ get; set; }
        [DataMember]
        public DateTime Fecha{ get; set; }

        public VentaCompleta(int id, string nombreApellido, string tipo, string estado, DateTime fecha)
        {
            Id = id;
            NombreApellido = nombreApellido;
            Tipo = tipo;
            Estado = estado;
            Fecha = fecha;
        }
    }
}
