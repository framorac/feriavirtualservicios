using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    [DataContract]
    public class Contratos
    {
        [DataMember]
        public int Id_contrato { get; set; }
        [DataMember]
        public int Id_usuario { get; set; }
        [DataMember]
        public DateTime Fecha_inicio { get; set; }
        [DataMember]
        public DateTime Fecha_termino { get; set; }

        public Contratos(int id_contrato, int id_usuario, DateTime fecha_inicio, DateTime fecha_termino)
        {
            Id_contrato = id_contrato;
            Id_usuario = id_usuario;
            Fecha_inicio = fecha_inicio;
            Fecha_termino = fecha_termino;
        }
    }
}
