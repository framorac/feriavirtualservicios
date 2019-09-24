using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FeriaVirtualServices.Structures
{
    public class AuxiliarFunctions
    {
        /// <summary>
        /// Función auxiliar que retorna un objeto genérico en formato JSON
        /// </summary>
        /// <param name="obj">Acepta cualquier tipo de datos</param>
        /// <returns>Retorna un string en formato JSON</returns>
        public string Return(object obj)
        {
            var serializer = new JavaScriptSerializer() { MaxJsonLength = 2147483644 };
            return serializer.Serialize(obj);
        }
    }
}
