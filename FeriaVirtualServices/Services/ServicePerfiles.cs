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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServicePerfiles" in both code and config file together.
    public class ServicePerfiles : IServicePerfiles
    {
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Perfiles> GetPerfiles()
        {
            List<Perfiles> datos = new List<Perfiles>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_perfiles.select_perfiles";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_perfiles", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipo = string.Empty;
                    string descripcion = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        tipo = reader[1].ToString();
                        descripcion = reader[2].ToString();
                        datos.Add(new Perfiles(id, tipo, descripcion));
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
