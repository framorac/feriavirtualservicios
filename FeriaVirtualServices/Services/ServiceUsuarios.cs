﻿using FeriaVirtualServices.Structures;
using FeriaVirtualServices.Structures.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
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
            bool r = false;
            try
            {
                Connection c = new Connection();
                string sql = "select * from usuarios where username='" + username + "' and password='" + password + "'";
                using (OracleCommand comm = new OracleCommand(sql, c.Conn))
                {
                    using (OracleDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            r = true;
                        }
                    }
                }
                c.Close();
            }
            catch (Exception e)
            {
                r = false;
                Debug.WriteLine(e.ToString());
            }


            return f.Return(r);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetUsuarios()
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
                comm.CommandText = "pkg_usuarios.select_usuarios";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("cur_employees", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    string usuario = string.Empty;
                    string perfil = string.Empty;
                    while (reader.Read())
                    {
                        usuario = reader[0].ToString();
                        perfil = reader[1].ToString();
                        datos.Add(new Usuario(usuario, "", perfil));
                    }
                }
                c.Close();
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
            return f.Return(datos);
        }
    }
}