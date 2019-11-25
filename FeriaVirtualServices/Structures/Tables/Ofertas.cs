using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Ofertas
    {
        [DataMember]
        public int Id_oferta { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Id_venta { get; set; }
        [DataMember]
        public DateTime Fecha_inicio { get; set; }
        [DataMember]
        public bool IsCertificado { get; set; }
        [DataMember]
        public bool IsEnvasado { get; set; }

        public Ofertas(int id_oferta, string username, int id_venta, DateTime fecha_inicio, bool isCertificado, bool isEnvasado)
        {
            Id_oferta = id_oferta;
            Username = username;
            Id_venta = id_venta;
            Fecha_inicio = fecha_inicio;
            IsCertificado = isCertificado;
            IsEnvasado = isEnvasado;
        }
    }
}
