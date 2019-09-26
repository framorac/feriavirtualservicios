using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    public class TipoEstado
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string descripcion { get; set; }

        public TipoEstado(int id, string tipo, string descripcion)
        {
            this.id = id;
            this.tipo = tipo;
            this.descripcion = descripcion;
        }
    }
}
