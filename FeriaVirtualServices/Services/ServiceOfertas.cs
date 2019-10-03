using FeriaVirtualServices.Structures;
using FeriaVirtualServices.Structures.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Services;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceOfertas" in both code and config file together.
    public class ServiceOfertas : IServiceOfertas
    {

        AuxiliarFunctions f = new AuxiliarFunctions();

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Ofertas> GetOfertas()
        {
            List<Ofertas> datos = new List<Ofertas>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ofertas.select_ofertas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_ofertas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string username = string.Empty;
                    int id_venta = 0;
                    DateTime fecha_inicio = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        username = reader[1].ToString();
                        id_venta = Convert.ToInt32(reader[2]);
                        fecha_inicio = Convert.ToDateTime(reader[3]);
                        datos.Add(new Ofertas(id, username, id_venta, fecha_inicio));
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            //return f.Return(datos);
            return datos;
        }
    }
}
