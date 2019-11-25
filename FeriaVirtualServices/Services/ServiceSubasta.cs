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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceSubasta" in both code and config file together.
    public class ServiceSubasta : IServiceSubasta
    {
        public List<Subastas> GetSubastas()
        {
            List<Subastas> subastas = new List<Subastas>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                comm.CommandText = "pkg_subastas.select_subastas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_subastas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string username = string.Empty;
                    int idOferta = 0;
                    DateTime fecha_inicio = DateTime.Now;
                    bool isCertificado = false;
                    bool isRefrigerado = false;
                    int capacidadCarga = 0;
                    string tipoTransporte = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        username = reader[1].ToString();
                        idOferta = Convert.ToInt32(reader[2]);
                        fecha_inicio = (DateTime)(reader[3]);
                        isCertificado = Convert.ToBoolean(reader[4]);
                        isRefrigerado = Convert.ToBoolean(reader[5]);
                        capacidadCarga = Convert.ToInt32(reader[6]);
                        tipoTransporte = reader[7].ToString();
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return subastas;
        }
    }
}
