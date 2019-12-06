using FeriaVirtualServices.Structures;
using FeriaVirtualServices.Structures.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Services;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceSubasta" in both code and config file together.
    public class ServiceSubasta : IServiceSubasta
    {
        AuxiliarFunctions f = new AuxiliarFunctions();

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
                    int precio = 0;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        username = reader[1].ToString();
                        idOferta = Convert.ToInt32(reader[2]);
                        fecha_inicio = (DateTime)(reader[3]);
                        if (reader[4].ToString() == "1") {
                            isCertificado = true;
                        }
                        if (reader[5].ToString() == "1") {
                            isRefrigerado = true;
                        }
                        capacidadCarga = Convert.ToInt32(reader[6]);
                        tipoTransporte = reader[7].ToString();
                        precio = Convert.ToInt32(reader[8]);

                        subastas.Add(new Subastas(id, username, idOferta, fecha_inicio, isCertificado, isRefrigerado, capacidadCarga, tipoTransporte, precio));
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateSubastaGanadora(int id_subasta)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_subastas.update_subasta_ganadora";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_subasta", OracleDbType.Int32, 38, "id_subasta").Value = id_subasta;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "La subasta ha sido actualizada";
                }
                else
                {
                    r = "No se ha actualizado ninguna subasta";
                }

                c.Close();
            }
            catch (Exception e)
            {
                r = "Ha ocurrido un error";
                Debug.WriteLine(e.ToString());
            }
            return f.Return(r);
        }
    }
}
