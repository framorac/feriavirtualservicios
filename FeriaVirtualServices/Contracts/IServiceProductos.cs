using FeriaVirtualServices.Structures.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceProductos" in both code and config file together.
    [ServiceContract]
    public interface IServiceProductos
    {
        [OperationContract]
        List<Productos> GetProductos();

        [OperationContract]
        string UpdateProducto(int id, string nombre, string descripcion, int id_tipoproducto);

        [OperationContract]
        string InsertProducto(string nombre, string descripcion, int id_tipoproducto);

        [OperationContract]
        string DeleteProducto(int id);
    }
}
