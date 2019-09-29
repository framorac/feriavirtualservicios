using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceVentas" in both code and config file together.
    [ServiceContract]
    public interface IServiceVentas
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetVentas();

        [OperationContract]
        string UpdateVenta(int id, DateTime fecha, int fk_tipoEstado);

        [OperationContract]
        string InsertVenta(int fk_usuario, DateTime fecha, int fk_tipoEstado, int fk_tipoVenta);

        [OperationContract]
        string DeleteVenta(int id);
    }
}
