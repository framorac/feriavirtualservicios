using FeriaVirtualServices.Structures.Tables;
using System.Collections.Generic;
using System.ServiceModel;

namespace FeriaVirtualServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISUsuarios" in both code and config file together.
    [ServiceContract]
    public interface IUsuarios
    {
        [OperationContract]
        string Login(string username, string password);

        [OperationContract]
        List<Usuario> GetUsuarios();

        [OperationContract]
        string UpdateUsuario(int id, string username, string password, int fk_perfil);

        [OperationContract]
        string InsertUsuario(string username, string password, int fk_perfil);

        [OperationContract]
        string DeleteUsuario(int id);
    }
}
