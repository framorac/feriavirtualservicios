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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceDetalleVenta" in both code and config file together.
    public class ServiceDetalleVenta : IServiceDetalleVenta
    {
        AuxiliarFunctions f = new AuxiliarFunctions();

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<DetalleVenta> GetDetalleVenta(int id_detalleVenta, int id_usuario)
        {
            List<DetalleVenta> datos = new List<DetalleVenta>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_detalle_venta.select_detalle_venta";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id_detalleVenta;
                comm.Parameters.Add("in_id_usuario", OracleDbType.Int32, 38, "id_usuario").Value = id_usuario;
                comm.Parameters.Add("cur_detalle_venta", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string nombre_producto = string.Empty;
                    int id_venta = 0;
                    int cantidad = 0;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre_producto = reader[1].ToString();
                        id_venta = Convert.ToInt32(reader[2]);
                        cantidad = Convert.ToInt32(reader[3]);
                        datos.Add(new DetalleVenta(id, nombre_producto, id_venta, cantidad));
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
        public string InsertDetalleVenta(int id_producto, int id_venta, int cantidad)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_detalle_venta.insert_detalle_venta";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_producto", OracleDbType.Int32, 38, "id_usuario").Value = id_producto;
                comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id_venta;
                comm.Parameters.Add("in_cantidad", OracleDbType.Int32, 38, "cantidad").Value = cantidad;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);
                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Detalle de venta ingresado.";
                }
                else
                {
                    r = "Detalle de venta no ha sido ingresada. Consulte con el equipo técnico.";
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
