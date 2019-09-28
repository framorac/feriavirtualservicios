﻿using System;
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
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetVentas/")]
        string GetVentas();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateVenta/?id={id}&fecha={fecha}&fk_tipoEstado={fk_tipoEstado}")]
        string UpdateVenta(int id, DateTime fecha, int fk_tipoEstado);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "/InsertVenta/?fk_usuario={fk_usuario}&fecha={fecha}&fk_tipoEstado={fk_tipoEstado}&fk_tipoVenta={fk_tipoVenta}")]
        string InsertVenta(int fk_usuario, DateTime fecha, int fk_tipoEstado, int fk_tipoVenta);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DeleteVenta/?id={id}")]
        string DeleteVenta(int id);
    }
}
