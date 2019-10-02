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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceProductos" in both code and config file together.
    public class ServiceProductos : IServiceProductos
    {
        AuxiliarFunctions f = new AuxiliarFunctions();

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Productos> GetProductos()
        {
            List<Productos> datos = new List<Productos>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_productos.select_productos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_productos", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string nombre = string.Empty;
                    string descripcion = string.Empty;
                    string tipoProducto = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        descripcion = reader[2].ToString();
                        tipoProducto = reader[3].ToString();
                        datos.Add(new Productos(id, nombre, descripcion, tipoProducto));
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
        public string UpdateProducto(int id, string nombre, string descripcion, int id_tipoproducto)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_productos.update_productos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_producto", OracleDbType.Int32, 38, "id_producto").Value = id;
                comm.Parameters.Add("in_nombre", OracleDbType.Varchar2, 50, "nombre").Value = nombre;
                comm.Parameters.Add("in_descripcion", OracleDbType.Varchar2, 200, "descripcion").Value = descripcion;
                comm.Parameters.Add("in_id_tipoproducto", OracleDbType.Int32, 38, "id_tipoproducto").Value = id_tipoproducto;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "El producto ha sido actualizada";
                }
                else
                {
                    r = "No se ha actualizado ningún producto";
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
        public string InsertProducto(string nombre, string descripcion, int id_tipoproducto)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_productos.insert_productos" +
                    "";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_nombre", OracleDbType.Varchar2, 50, "nombre").Value = nombre;
                comm.Parameters.Add("in_descripcion", OracleDbType.Varchar2, 200, "descripcion").Value = descripcion;
                comm.Parameters.Add("in_id_tipoproducto", OracleDbType.Int32, 38, "in_id_tipoproducto").Value = id_tipoproducto; 
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Producto Ingresado";
                }
                else
                {
                    r = "Producto no ha sido ingresado. Consulte con el equipo técnico.";
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
        public string DeleteProducto(int id)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;

                comm.CommandText = "pkg_productos.delete_productos";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id_producto").Value = id;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Producto eliminado.";
                }
                else
                {
                    r = "Producto no existe.";
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
