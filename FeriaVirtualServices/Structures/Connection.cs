using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeriaVirtualServices.Structures
{
    public class Connection
    {
        public OracleConnection Conn { get; set; }

        /// <summary>
        /// Constructor que realiza la conexión a la base de datos de oracle.
        /// Para cerrarla, puede usar el método Close() de la misma clase.
        /// </summary>
        public Connection() {
            try
            {
                //string connectionString = "User Id = system; Password = 123; Data Source=localhost";
                string connectionString = "User Id = duocadmin; Password = gEB8UG8zpp8xY8TFv6Ly; Data Source=(DESCRIPTION=" +
                    "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=feriavirtual.cr9n8uykvimk.us-east-2.rds.amazonaws.com)" +
                    "(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));";
                // Instalar el paquete NUGET Oracle.ManagedDataAccess
                Conn = new OracleConnection(connectionString);
                Conn.Open();
                Debug.WriteLine("Conexión abierta a Oracle: " + Conn.ServerVersion);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Se ha caído la conexión");
                Debug.WriteLine(e.ToString());
            }
        }

        public void Close()
        {
            Conn.Clone();
            Conn.Dispose();
        }

    }
}
