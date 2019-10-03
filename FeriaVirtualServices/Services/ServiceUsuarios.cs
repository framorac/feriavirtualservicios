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
// Añadir referencia de servicio System.Web.Extensions
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;


namespace FeriaVirtualServices.Services
{
    public class ServiceUsuarios : IUsuarios
    {
        AuxiliarFunctions f = new AuxiliarFunctions();
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Login(string username, string password)
        {
            string[] valores = new string[2];
            bool isFill = false;
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                // Otro: https://stackoverflow.com/questions/3940587/calling-oracle-stored-procedure-from-c
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_usuariosv2.login_usuario";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_username", OracleDbType.Varchar2, 30, "username").Value = username;  
                comm.Parameters.Add("in_password", OracleDbType.Varchar2, 20, "password").Value = password;
                comm.Parameters.Add("t_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    string usuario = string.Empty;
                    string perfil = string.Empty;
                    while (reader.Read())
                    {
                        usuario = reader[0].ToString();
                        perfil = reader[1].ToString();
                        valores[0] = usuario;
                        valores[1] = perfil;
                        isFill = true;
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return isFill ? isFill.ToString() : f.Return("Usuario y/o contraseño no válidos");
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Usuario> GetUsuarios()
        {
            List<Usuario> datos = new List<Usuario>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_usuariosv2.select_usuarios";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_usuarios", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    string usuario = string.Empty;
                    string password = string.Empty;
                    string perfil = string.Empty;
                    string nombre = string.Empty;
                    string apellido = string.Empty;
                    string email = string.Empty;
                    DateTime fecha = DateTime.Now;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0].ToString());
                        usuario = reader[1].ToString();
                        password = reader[2].ToString();
                        perfil = reader[3].ToString();
                        nombre = reader[4].ToString();
                        apellido = reader[5].ToString();
                        email = reader[6].ToString();
                        fecha = (DateTime)reader[7];
                        datos.Add(new Usuario(id, usuario, password, perfil, nombre, apellido, email, fecha));
                    }
                }
                c.Close();
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
            return datos;
            //return f.Return(datos);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateUsuario(int id, string username, string password, int fk_perfil, string nombre, string apellido, string email, DateTime fecha) {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                // Otro: https://stackoverflow.com/questions/3940587/calling-oracle-stored-procedure-from-c
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_usuariosv2.update_usuario";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38 , "id").Value = id;
                comm.Parameters.Add("in_username", OracleDbType.Varchar2, 30, "username").Value = username;
                comm.Parameters.Add("in_password", OracleDbType.Varchar2, 20, "password").Value = password;
                comm.Parameters.Add("in_id_perfil", OracleDbType.Int32, 38, "id_perfil").Value = fk_perfil;
                comm.Parameters.Add("in_nombre", OracleDbType.Varchar2, 50, "nombre").Value = nombre;
                comm.Parameters.Add("in_apellido", OracleDbType.Varchar2, 50, "apellido").Value = apellido;
                comm.Parameters.Add("in_email", OracleDbType.Varchar2, 150, "email").Value = email;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 200, "fecha_creacion").Value = fecha;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "La fila ha sido actualizada";
                }
                else {
                    r = "No se ha actualizado ninguna fila";
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
        public string InsertUsuario(string username, string password, int id_perfil, string nombre, string apellido, string email, DateTime fecha)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                // Otro: https://stackoverflow.com/questions/3940587/calling-oracle-stored-procedure-from-c
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_usuariosv2.insert_usuario";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_username", OracleDbType.Varchar2, 30, "username").Value = username;
                comm.Parameters.Add("in_password", OracleDbType.Varchar2, 20, "password").Value = password;
                comm.Parameters.Add("in_id_perfil", OracleDbType.Int32, 38, "id_perfil").Value = id_perfil;
                comm.Parameters.Add("in_nombre", OracleDbType.Varchar2, 50, "nombre").Value = nombre;
                comm.Parameters.Add("in_apellido", OracleDbType.Varchar2, 50, "apellido").Value = apellido;
                comm.Parameters.Add("in_email", OracleDbType.Varchar2, 100, "email").Value = email;
                comm.Parameters.Add("in_fecha", OracleDbType.Date, 50, "fecha_creacion").Value = fecha;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Usuario Ingresado.";
                }
                else
                {
                    r = "Usuario no ha sido ingresado. Consulte con el equipo técnico.";
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

        public string DeleteUsuario(int id)
        {
            string r = string.Empty;
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                // Otro: https://stackoverflow.com/questions/3940587/calling-oracle-stored-procedure-from-c
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                // retorna usuario y perfil
                comm.CommandText = "pkg_usuariosv2.delete_usuario";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id", OracleDbType.Int32, 38, "id").Value = id;
                OracleParameter param = comm.Parameters.Add("response", OracleDbType.Int32, ParameterDirection.Output);

                comm.ExecuteNonQuery();
                var responseQuery = param.Value.ToString();
                if (responseQuery == "1")
                {
                    r = "Usuario eliminado.";
                }
                else
                {
                    r = "Usuario no existe.";
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