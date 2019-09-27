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

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceTipoEstados" in both code and config file together.
    public class ServiceTipoEstados : IServiceTipoEstados
    {
        AuxiliarFunctions f = new AuxiliarFunctions();

        public string GetTipoEstados()
        {
            List<TipoEstado> datos = new List<TipoEstado>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_tipoestados.select_tipoestados";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_tipoestados", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
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
                        datos.Add(new TipoEstado(id, tipo, descripcion));
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return f.Return(datos);
        }
    }
}
