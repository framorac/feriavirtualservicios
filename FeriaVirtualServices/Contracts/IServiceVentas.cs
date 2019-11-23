using FeriaVirtualServices.Structures.Tables;
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
        List<Ventas> GetVentas();        

        [OperationContract]
        string InsertVenta(int fk_usuario, DateTime fecha, int fk_tipoVenta);

        [OperationContract]
        string DeleteVenta(int id);

        [OperationContract]
        string UpdateVenta(int id_venta, int id_estado);

        [OperationContract]
        List<HistoricoEstadoVentas> GetHistóricoEstadoVentas();

        [OperationContract]
        List<VentaCompleta> GetVentaCompleta(int idTipoEstado, int idTipoVenta);
    }
}
