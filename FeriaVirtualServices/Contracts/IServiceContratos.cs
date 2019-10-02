using FeriaVirtualServices.Structures.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceContratos" in both code and config file together.
    [ServiceContract]
    public interface IServiceContratos
    {
        [OperationContract]
        List<Contratos> GetContratos();

        [OperationContract]
        string UpdateVenta(int id, DateTime fecha_inicio, DateTime fecha_termino);

        [OperationContract]
        string InsertContrato(int id_usuario, DateTime fecha_inicio, DateTime fecha_termino);

        [OperationContract]
        string DeleteContrato(int id_contrato);
    }
}
