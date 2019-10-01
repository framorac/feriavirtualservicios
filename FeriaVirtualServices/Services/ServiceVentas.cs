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
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Script.Services;

namespace FeriaVirtualServices.Services
{
    
    public class ServiceVentas : IServiceVentas
    {
        AuxiliarFunctions f = new AuxiliarFunctions();


        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Ventas> GetVentas()
        {
            List<Ventas> datos = new List<Ventas>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string username = string.Empty;
                    DateTime date = new DateTime();
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        username = reader[1].ToString();
                        date = (DateTime)(reader[2]);
                        tipoEstado = reader[3].ToString();
                        tipoVenta = reader[4].ToString();
                        datos.Add(new Ventas(id, username, date, tipoEstado, tipoVenta));
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
        public string UpdateVenta(int id, DateTime fecha, int fk_tipoEstado)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.update_ventas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id").Value = id;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 30, "fecha").Value = fecha;
                comm.Parameters.Add("in_fk_tipoestado", OracleDbType.Int32, 38, "fk_tipoestado").Value = fk_tipoEstado;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "La venta ha sido actualizada";
                }
                else
                {
                    r = "No se ha actualizado ninguna venta";
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
        public string InsertVenta(int fk_usuario, DateTime fecha, int fk_tipoEstado, int fk_tipoVenta)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.insert_ventas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_fk_usuario", OracleDbType.Int32, 38, "fk_usuario").Value = fk_usuario;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 30, "fecha").Value = fecha;
                comm.Parameters.Add("in_fk_tipoestado", OracleDbType.Int32, 38, "fk_tipoestado").Value = fk_tipoEstado;
                comm.Parameters.Add("in_fk_tipoventa", OracleDbType.Int32, 38, "fk_tipoventa").Value = fk_tipoVenta;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Venta Ingresada.";
                }
                else
                {
                    r = "Venta no ha sido ingresada. Consulte con el equipo técnico.";
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteVenta(int id)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                
                comm.CommandText = "pkg_ventas.delete_ventas";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id").Value = id;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Venta eliminada.";
                }
                else
                {
                    r = "Venta no existe.";
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
