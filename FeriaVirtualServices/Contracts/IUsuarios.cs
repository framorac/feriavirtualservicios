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
        string GetUsuarios();
    }
}
