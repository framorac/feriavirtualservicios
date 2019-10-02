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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceContratos" in both code and config file together.
    public class ServiceContratos : IServiceContratos
    {
        AuxiliarFunctions f = new AuxiliarFunctions();

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Contratos> GetContratos()
        {
            List<Contratos> datos = new List<Contratos>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_contratos.select_contratos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_contratos", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    int id_usuario = 0;
                    DateTime fecha_inicio = new DateTime();
                    DateTime fecha_termino = new DateTime();
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        id_usuario = Convert.ToInt32(reader[1]);
                        fecha_inicio = (DateTime)(reader[2]);
                        fecha_termino = (DateTime)(reader[3]);
                        datos.Add(new Contratos(id, id_usuario, fecha_inicio, fecha_termino));
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateVenta(int id, DateTime fecha_inicio, DateTime fecha_termino)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_contratos.update_contratos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id").Value = id;
                comm.Parameters.Add("in_fecha_inicio", OracleDbType.Date, 30, "fecha_inicio").Value = fecha_inicio;
                comm.Parameters.Add("in_fecha_termino", OracleDbType.Date, 30, "fecha_termino").Value = fecha_termino;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "El contrato ha sido actualizado";
                }
                else
                {
                    r = "No se ha actualizado ningún contrato";
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string InsertContrato(int id_usuario, DateTime fecha_inicio, DateTime fecha_termino)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_contratos.insert_contratos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_usuario", OracleDbType.Int32, 38, "id_usuario").Value = id_usuario;
                comm.Parameters.Add("in_fecha_inicio", OracleDbType.Date, 30, "fecha_inicio").Value = fecha_inicio;
                comm.Parameters.Add("in_fecha_termino", OracleDbType.Date, 30, "fecha_termino").Value = fecha_termino;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Contrato Ingresado.";
                }
                else
                {
                    r = "Contrato no ha sido ingresado. Consulte con el equipo técnico.";
                }

                c.Close();
            }
            catch (Exception e)
            {
                r = "Ha ocurrido un error.";
                Debug.WriteLine(e.ToString());
            }
            return f.Return(r);
        }

        public string DeleteContrato(int id_contrato) {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;

                comm.CommandText = "pkg_contratos.delete_contratos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_contrato", OracleDbType.Int32, 38, "id").Value = id_contrato;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Contrato eliminado.";
                }
                else
                {
                    r = "Contrato no existe.";
                }

                c.Close();
            }
            catch (Exception e)
            {
                r = "Ha ocurrido un error.";
                Debug.WriteLine(e.ToString());
            }
            return f.Return(r);
        }
    }
}
