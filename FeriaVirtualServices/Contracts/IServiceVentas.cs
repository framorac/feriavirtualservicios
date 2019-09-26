using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceVentas" in both code and config file together.
    [ServiceContract]
    public interface IServiceVentas
    {
        [OperationContract]
        string GetVentas();

        [OperationContract]
        string UpdateVenta(int id, DateTime fecha, int fk_tipoEstado);
    }
}
