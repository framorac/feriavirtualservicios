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
using System.Web.Script.Services;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceDetalleOferta" in both code and config file together.
    public class ServiceDetalleOferta : IServiceDetalleOferta
    {
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<DetalleOferta> GetDetalleOfertas(int idOferta)
        {
            List<DetalleOferta> datos = new List<DetalleOferta>();
            try
            {
                Connection c = new Connection();
                // En base de este documento: https://www.c-sharpcorner.com/article/calling-oracle-stored-procedures-from-microsoft-net/
                OracleDataAdapter adapter = new OracleDataAdapter();
                OracleCommand comm = new OracleCommand();
                comm.Connection = c.Conn;
                comm.CommandText = "pkg_detalle_oferta.select_detalle_oferta";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.Add("in_id_oferta", OracleDbType.Int32, 38, "id_oferta").Value = idOferta;
                comm.Parameters.Add("cur_detalle_oferta", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                using (OracleDataReader reader = comm.ExecuteReader())
                {
                    int id = 0;
                    int id_oferta = 0;
                    string nombre = string.Empty;
                    int cantidad = 0;
                    int precio = 0;
                    int? idProducto = 0;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader[0]);
                        id_oferta = Convert.ToInt32(reader[1]);
                        nombre = reader[2].ToString();
                        cantidad = Convert.ToInt32(reader[3]);
                        precio = Convert.ToInt32(reader[4]);
                        idProducto = Convert.ToInt32(reader[5]);
                        datos.Add(new DetalleOferta(id, id_oferta, nombre, cantidad, precio, idProducto));
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
