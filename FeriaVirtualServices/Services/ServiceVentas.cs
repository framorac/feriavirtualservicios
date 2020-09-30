using FeriaVirtualServices.Structures;
using FeriaVirtualServices.Structures.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
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
        public string UpdateVenta(int id_estado, int id_venta, char isLocal = '0')
        {
            ServiceVentas sv = new ServiceVentas();
            ServiceUsuarios su = new ServiceUsuarios();

            var venta = sv.GetVentas().Where(x => x.id == id_venta).FirstOrDefault();
            var email = su.GetUsuarios().Where(x => x.Username == venta.username).FirstOrDefault().Email;
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                OracleParameter param = new OracleParameter();
                if (isLocal != '1')
                {
                    comm.CommandText = "pkg_historico_estado_ventas.insert_hev";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = id_estado;
                    comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id_venta;
                    comm.Parameters.Add("in_fecha", OracleDbType.Date, 100, "fecha").Value = DateTime.Now;
                    param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);
                }
                else {
                    comm.CommandText = "pkg_historico_estado_ventas.insert_hev_local";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = id_estado;
                    comm.Parameters.Add("in_id_venta", OracleDbType.Int32, 38, "id_venta").Value = id_venta;
                    comm.Parameters.Add("in_fecha", OracleDbType.Date, 100, "fecha").Value = DateTime.Now;
                    comm.Parameters.Add("in_islocal", OracleDbType.Char, 1, "islocal").Value = isLocal;
                    param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);
                }
                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "La venta ha sido actualizada";
                    if (id_estado == 1)
                    {
                        //Cuerpo del correo
                        LinkedResource img =
                        new LinkedResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\IMAGENES\Frutas.jpg", MediaTypeNames.Image.Jpeg);
                        img.ContentId = "imagen";
                        StringBuilder mensaje = new StringBuilder();

                        mensaje.AppendFormat("<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>");
                        mensaje.AppendFormat("<tr>");
                        mensaje.AppendFormat("<td style='padding: 0'>");
                        mensaje.AppendFormat("<img style='padding: 0; display: block' src='cid:imagen' width='600px' height='100px'>");
                        mensaje.AppendFormat("</td>");
                        mensaje.AppendFormat("</tr>");
                        mensaje.AppendFormat("<tr>");
                        mensaje.AppendFormat("<td style='background - color: #ecf0f1'>");
                        mensaje.AppendFormat("<div style='color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>");
                        mensaje.AppendFormat("<h2 style='color: #e67e22; margin: 0 0 7px'>Hola!</h2>");
                        mensaje.AppendFormat("<p style='margin: 2px; font - size: 15px'>Su solicitud número {0} fue ingresada para iniciar el proceso de ofertas de productores.</p>", venta.id);
                        mensaje.AppendFormat("<p style='color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0'>Maipo Grande 2019</p>");
                        mensaje.AppendFormat("</div>");
                        mensaje.AppendFormat("</td>");
                        mensaje.AppendFormat("</tr>");
                        mensaje.AppendFormat("</table>");
                        EnviarCorreo(email, "Solicitud ingresada", mensaje.ToString(), img);
                    }
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
                    bool activo = false;
                    bool isLocal = false;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        tipoEstado = reader[1].ToString();
                        id_venta = Convert.ToInt32(reader[2]);
                        fecha = (DateTime)(reader[3]);
                        if (reader[4].ToString() == "1")
                        {
                            activo = true;
                        }
                        else
                        {
                            activo = false;
                        }
                        if (reader[5].ToString() == "1")
                        {
                            isLocal = true;
                        }
                        else
                        {
                            isLocal = false;
                        }

                        datos.Add(new HistoricoEstadoVentas(id, tipoEstado, id_venta, fecha, activo, isLocal));
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
        public List<VentaCompleta> GetVentaCompleta(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos;
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoAbierto(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }


                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "abierto").ToList();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoIngresada(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }


                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "ingresada").ToList();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoEnSubasta(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }


                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "en subasta").ToList();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoEnCamino(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }


                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "en camino").ToList();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoRecepcionado(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "recepcionado").ToList();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<VentaCompleta> GetVentaCompletaFiltradoFinalizada(int idTipoEstado, int idTipoVenta)
        {
            List<VentaCompleta> datos = new List<VentaCompleta>();
            try
            {
                Connection c = new Connection();
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_ventas.select_ventas_completa";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_estado", OracleDbType.Int32, 38, "id_estado").Value = idTipoEstado;
                comm.Parameters.Add("in_id_tipoventa", OracleDbType.Int32, 38, "id_tipoventa").Value = idTipoVenta;
                comm.Parameters.Add("cur_ventas", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string tipoEstado = string.Empty;
                    string tipoVenta = string.Empty;
                    string nombre = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        nombre = reader[1].ToString();
                        tipoVenta = reader[2].ToString();
                        tipoEstado = reader[3].ToString();
                        fecha = (DateTime)(reader[4]);
                        datos.Add(new VentaCompleta(id, nombre, tipoVenta, tipoEstado, fecha));
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return datos.Where(x => x.Estado == "finalizada").ToList();
        }

        private bool EnviarCorreo(string correoDestinatario, string asunto, string cuerpo, LinkedResource lr)
        {
            bool r = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.UseDefaultCredentials = false;
                mail.IsBodyHtml = true;

                mail.From = new MailAddress("equipomaipogrande@gmail.com");
                mail.To.Add(correoDestinatario);
                mail.Subject = asunto;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                cuerpo, null, "text/html");
                htmlView.LinkedResources.Add(lr);
                mail.AlternateViews.Add(htmlView);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("equipomaipogrande", "duocadmin123");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                r = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                r = false;
            }

            return r;
        }
    }
}
