using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures.Tables
{
    public class Ventas
    {
        public int id { get; set; }
        public string username { get; set; }
        public DateTime fecha { get; set; }
        public string tipoEstado { get; set; }
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
