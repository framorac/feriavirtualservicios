using FeriaVirtualServices.Structures.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceDetalleVenta" in both code and config file together.
    [ServiceContract]
    public interface IServiceDetalleVenta
    {
        [OperationContract]
        List<DetalleVenta> GetDetalleVenta(int id_detalleVenta, int id_usuario);

        [OperationContract]
        string InsertDetalleVenta(int id_producto, int id_venta, int cantidad);
    }
}
