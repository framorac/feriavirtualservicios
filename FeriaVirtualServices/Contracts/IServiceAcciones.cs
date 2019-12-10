using FeriaVirtualServices.Structures.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceAcciones" in both code and config file together.
    [ServiceContract]
    public interface IServiceAcciones
    {
        [OperationContract]
        Usuario AdjudicarProductor(int idVenta);

        [OperationContract]
        Usuario AdjudicarTransportista(int idVenta);

        [OperationContract]
        string ComenzarProcesoLocal(int idVentaFinalizada);


    }
}
