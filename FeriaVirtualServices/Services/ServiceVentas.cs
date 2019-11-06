﻿using FeriaVirtualServices.Structures;
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
        //Método que obtiene todas las ventas
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
                    string tipoVenta = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        username = reader[1].ToString();
                        date = (DateTime)(reader[2]);
                        tipoVenta = reader[3].ToString();
                        datos.Add(new Ventas(id, username, date, tipoVenta));
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
        public string InsertVenta(int fk_usuario, DateTime fecha, int fk_tipoVenta)
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
                comm.Parameters.Add("in_id_usuario", OracleDbType.Int32, 38, "id_usuario").Value = fk_usuario;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 30, "fecha").Value = fecha;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = fk_tipoVenta;
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
                comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id;
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

        /// <summary>
        /// Esta función puede ser confunsa; en realidad realiza un insert en la tabla histórico_estado_ventas
        /// Lo cual realiza un seguimiento para cada cambio de estado, con un afecha automática, sin tener
        /// que ingresarlo como parámetros de la vista de usuario.
        /// Tener cuidado que la tabla de ventas, ya no recibe la clave foránea del tipo de estado, ahora esta tabla es
        /// la que la envia al histórico de cambios de estado.
        /// </summary>
        /// <param name="id_venta">Id de la venta que se quiere actualizar</param>
        /// <param name="id_estado">Id del estado al cual se queire actualizar</param>
        /// <returns></returns>
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateVenta(int id_estado, int id_venta)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_historico_estado_ventas.insert_hev";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = id_estado;
                comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id_venta;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 30, "fecha").Value = DateTime.Now;
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
        public List<HistoricoEstadoVentas> GetHistóricoEstadoVentas()
        {
            List<HistoricoEstadoVentas> datos = new List<HistoricoEstadoVentas>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_historico_estado_ventas.select_hev";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_hev", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    int id_venta = 0;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        tipoEstado = reader[1].ToString();
                        id_venta = Convert.ToInt32(reader[2]);
                        fecha = (DateTime)(reader[3]);
                        datos.Add(new HistoricoEstadoVentas(id, tipoEstado, id_venta, fecha));
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

        /// <summary>
        /// Método que devuelve una consulta específica de acuerdo al parámetros ingresado.
        /// </summary>
        /// <param name="idVenta">id de la venta</param>
        /// <param name="tipoConsulta">si es 0 corresponde devuelve nombre y apellido, 1 tipo de estado y 2 tipo de venta</param>
        /// <returns></returns>
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<GenericString> GetVentaEspecífica(int idVenta, int tipoConsulta)
        {
            List<GenericString> datos = new List<GenericString>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;

                switch (tipoConsulta) {
                    case 0:
                        comm.CommandText = "pkg_ventas.select_ventas_user";
                        break;
                    case 1:
                        comm.CommandText = "pkg_ventas.select_ventas_estado";
                        break;
                    case 2:
                        comm.CommandText = "pkg_ventas.select_ventas_tipo";
                        break;
                    default:
                        return datos;

                }
               
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id_venta").Value = idVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string generic = string.Empty;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        generic = reader[1].ToString();
                        datos.Add(new GenericString(id, generic));
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
