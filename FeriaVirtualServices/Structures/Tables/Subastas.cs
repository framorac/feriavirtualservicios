using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Subastas
    {
        [DataMember]
        public int Id_subasta { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Id_oferta { get; set; }
        [DataMember]
        public DateTime Fecha_inicio { get; set; }
        [DataMember]
        public bool IsCertificado { get; set; }
        [DataMember]
        public bool IsRefrigerado { get; set; }
        [DataMember]
        public int CapacidadCarga { get; set; }
        [DataMember]
        public string TipoTransporte { get; set; }
        [DataMember]
        public int Precio { get; set; }


        public Subastas(int id_subasta, string username, int id_oferta, DateTime fecha_inicio, bool isCertificado, bool isRefrigerado, int capacidadCarga, string tipoTransporte, int precio)
        {
            Id_subasta = id_subasta;
            Username = username;
            Id_oferta = id_oferta;
            Fecha_inicio = fecha_inicio;
            IsCertificado = isCertificado;
            IsRefrigerado = isRefrigerado;
            CapacidadCarga = capacidadCarga;
            TipoTransporte = tipoTransporte;
            Precio = precio;
        }
    }
}
