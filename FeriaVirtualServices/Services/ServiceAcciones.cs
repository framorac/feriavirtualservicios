using FeriaVirtualServices.Structures.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Script.Services;

namespace FeriaVirtualServices.Services
{
    public class ServiceAcciones : IServiceAcciones
    {

        /// <summary>
        /// Sólo se debe ingresar el id de la venta que se quiere encontrar al productor que tenga la mejor oferta
        /// El criterio se explica en los procedimientos ya definidos en la documentación
        /// </summary>
        /// <param name="idVenta">Id de la venta</param>
        /// <returns>Retorna un usuario, en caso de que no encuentre, se devolverá un usuario vacío (sin atributos definidos).</returns>
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Usuario AdjudicarProductor(int idVenta) {
            Usuario usuario = new Usuario();

            ServiceOfertas so = new ServiceOfertas();
            ServiceVentas sv = new ServiceVentas();
            ServiceUsuarios su = new ServiceUsuarios();
            ServiceDetalleOferta sdo = new ServiceDetalleOferta();
            int idOfertaGanadora = 0;
            var ofertas = so.GetOfertas();
            ofertas.Where(x => x.Id_venta == idVenta);
            var tieneOfertas = ofertas.Where(x => x.Id_venta == idVenta).ToList();
            var venta = sv.GetVentas().Where(x => x.id == idVenta).FirstOrDefault();
            var email = su.GetUsuarios().Where(x => x.Username == venta.username).FirstOrDefault().Email;
            
            // validamos si ya está en un proceso posterior.
            var estadoVenta = sv.GetHistóricoEstadoVentas().Where(x => x.Id_venta == venta.id && x.Activo).FirstOrDefault().TipoEstado;
            if (estadoVenta != "abierto") {
                return usuario;
            }

            if (tieneOfertas.Count == 0)
            {
                // Notificar al cliente que no tiene adjudicaciones
                LinkedResource img =
                new LinkedResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\IMAGENES\Frutas.jpg", MediaTypeNames.Image.Jpeg);
                StringBuilder mensaje = new StringBuilder();
                img.ContentId = "imagen";

                mensaje.AppendFormat("<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='padding: 0'>");
                mensaje.AppendFormat("<img style='padding: 0; display: block' src='cid:imagen' width='600px' height='100px'>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='background - color: #ecf0f1'>");
                mensaje.AppendFormat("<div style='color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>");
                mensaje.AppendFormat("<h2 style='color: #e67e22; margin: 0 0 7px'>Hola!</h2>");
                mensaje.AppendFormat("<p style='margin: 2px; font - size: 15px'>Su solicitud ha sido <b style='color: #FF0000'>cancelada</b>, debido a que no se recibieron ofertas por parte de productores.</p>");
                mensaje.AppendFormat("<p style='color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0'>Maipo Grande 2019</p>");
                mensaje.AppendFormat("</div>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("</table>");
                EnviarCorreo(email, "Solicitud Cancelada", mensaje.ToString(), img);
                sv.UpdateVenta(5, idVenta);
                
            }
            else {
                // Aquí empezamos el algoritmo para adjudicar al productor ganador
                // Esta tupla va a contener un consolidado de la oferta, que nos permitirá encontrar al ganador de forma eficiente
                // Y que iremos poblando a medida que encuentre el criterio
                // Primer criterio -> menor precio. Segundo Criterio -> Mejor calidad. Tercer criterio: " Oferta más temprana".
                // ITEM1: Id de la oferta
                // ITEM2: Precio total de la oferta
                // ITEM3: Username de la oferta
                // ITEM4: total de la calidad
                // ITEM5: fecha que se ingresó la oferta
                List<Tuple<int, int, string, int, DateTime?>> ofertasMenorPrecio = new List<Tuple<int, int, string, int, DateTime?>>();
                foreach (var oferta in ofertas)
                {

                    List<DetalleOferta> detallesFiltrados = sdo.GetDetalleOfertas(oferta.Id_oferta);
                    var total = -1;
                    // Guardamos el total de la oferta por todos los productos dentro de la oferta del productor.
                    foreach (var detalle in detallesFiltrados)
                    {
                        total += detalle.Precio * detalle.Cantidad;
                    }

                    // Aprovechamos de guardar los parámetros de calidad y su fecha.
                    var calidad = 0;
                    calidad = oferta.IsCertificado ? 1 : 0;
                    calidad += oferta.IsEnvasado ? 1 : 0;

                    // Este es para el primer caso (no hay ofertas ganadoras)
                    if (ofertasMenorPrecio.Count == 0)
                    {
                        Tuple<int, int, string, int, DateTime?> ofertaMenor = new Tuple<int, int, string, int, DateTime?>(oferta.Id_oferta, total, oferta.Username, calidad, oferta.Fecha_inicio);
                        ofertasMenorPrecio.Add(ofertaMenor);
                    }
                    else {
                        Tuple<int, int, string, int, DateTime?> ofertaMenor = new Tuple<int, int, string, int, DateTime?>(-1,-1, string.Empty, -1, null);
                        foreach (var omp in ofertasMenorPrecio)
                        {
                            if (total != -1 && omp.Item2 >= total)
                            {
                                ofertaMenor = new Tuple<int, int, string, int, DateTime?>(oferta.Id_oferta, total, oferta.Username, calidad, oferta.Fecha_inicio);
                                break;
                            }
                        }
                        if (ofertaMenor.Item1 != -1 && ofertaMenor.Item2 != -1) {
                            if (ofertasMenorPrecio.FirstOrDefault().Item2 > total) {
                                ofertasMenorPrecio.Clear();
                            }
                            ofertasMenorPrecio.Add(ofertaMenor);
                        }
                    }
                }
                // Único ganador
                if (ofertasMenorPrecio.Count == 1)
                {
                    usuario = su.GetUsuarios().Where(x => x.Username == ofertasMenorPrecio.FirstOrDefault().Item3).FirstOrDefault();
                    idOfertaGanadora = ofertasMenorPrecio.FirstOrDefault().Item1;
                }
                // En caso de que hayan 2 o más ofertas con el mismo precio, seguimos con el siguiente criterio de selección
                else
                {
                    // Buscamos la oferta con mayores parámetros de calidad
                    // la lígica es ordernar bajo el item4 (total de calidad) de forma descendiente y escoger aquellos que sean igual al máximo
                    var ofertasMejorCalidad = ofertasMenorPrecio.OrderByDescending(x => x.Item4).Where(y => y.Item4 == ofertasMenorPrecio.Max(z => z.Item4)).ToList();
                    // Si hay uno sólo con este criterio
                    if (ofertasMejorCalidad.Count == 1)
                    {
                        usuario = su.GetUsuarios().Where(x => x.Username == ofertasMejorCalidad.FirstOrDefault().Item3).FirstOrDefault();
                        idOfertaGanadora = ofertasMejorCalidad.FirstOrDefault().Item1;
                    }
                    // Caso último que tengamos más de una oferta con la misma calidad, entonces escogemos por la oferta más temprana
                    else {
                        var primeraOferta = ofertasMejorCalidad.OrderBy(x => x.Item5).FirstOrDefault();
                        usuario = su.GetUsuarios().Where(x => x.Username == primeraOferta.Item3).FirstOrDefault();
                        idOfertaGanadora = primeraOferta.Item1;
                    }
                }

                LinkedResource img =
                new LinkedResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\IMAGENES\Frutas.jpg", MediaTypeNames.Image.Jpeg);
                img.ContentId = "imagen";
                StringBuilder mensaje = new StringBuilder();

                mensaje.AppendFormat("<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='padding: 0'>");
                mensaje.AppendFormat("<img style='padding: 0; display: block' src='cid:imagen' width='600px' height='100px'>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='background - color: #ecf0f1'>");
                mensaje.AppendFormat("<div style='color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>");
                mensaje.AppendFormat("<h2 style='color: #e67e22; margin: 0 0 7px'>Hola!</h2>");
                mensaje.AppendFormat("<p style='margin: 2px; font - size: 15px'>Su solicitud ya tiene un productor ganador, cuyos datos son. Nombre: {0}, Apellido: {0}. Ahora su solicitud ha pasado al estado <b>En subasta</b>, para la subasta de transporte.</p>", usuario.Nombre, usuario.Apellido);
                mensaje.AppendFormat("<p style='color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0'>Maipo Grande 2019</p>");
                mensaje.AppendFormat("</div>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("</table>");
                EnviarCorreo(email, "Solicitud se encuentra en subasta", mensaje.ToString(), img);
                sv.UpdateVenta(2, idVenta);
                so.UpdateOfertaGanadora(idOfertaGanadora);
            }

            return usuario;
        }

        /// <summary>
        /// La lógica es similar a la adjudicación de productor
        /// Se podría incorporar otro criterio por capacidad, pero se omite para el caso, para simplicar el problema.
        /// </summary>
        /// <param name="idVenta"></param>
        /// <returns>Retorna el usuario que se ha adjudicado la oferta/venta</returns>
        /// [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Usuario AdjudicarTransportista(int idVenta)
        {
            Usuario usuario = new Usuario();
            ServiceSubasta ss = new ServiceSubasta();
            ServiceVentas sv = new ServiceVentas();
            ServiceUsuarios su = new ServiceUsuarios();
            var venta = sv.GetVentas().Where(x => x.id == idVenta).FirstOrDefault();
            var email = su.GetUsuarios().Where(x => x.Username == venta.username).FirstOrDefault().Email;
            var subastas = ss.GetSubastas();
            int idSubastaGanadora = 0;
            // validamos si ya está en un proceso posterior.
            var estadoVenta = sv.GetHistóricoEstadoVentas().Where(x => x.Id_venta == venta.id && x.Activo).FirstOrDefault().TipoEstado;
            if (estadoVenta != "en subasta")
            {
                return usuario;
            }

            if (subastas.Count == 0)
            {
                LinkedResource img =
                new LinkedResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\IMAGENES\Frutas.jpg", MediaTypeNames.Image.Jpeg);
                img.ContentId = "imagen";
                StringBuilder mensaje = new StringBuilder();

                mensaje.AppendFormat("<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='padding: 0'>");
                mensaje.AppendFormat("<img style='padding: 0; display: block' src='cid:imagen' width='600px' height='100px'>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='background - color: #ecf0f1'>");
                mensaje.AppendFormat("<div style='color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>");
                mensaje.AppendFormat("<h2 style='color: #e67e22; margin: 0 0 7px'>Hola!</h2>");
                mensaje.AppendFormat("<p style='margin: 2px; font - size: 15px'>Su solicitud no ha tenido subastas de transporte, se realizará un nuevo proceso de subasta, agradecemos su comprensión</p>");
                mensaje.AppendFormat("<p style='color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0'>Maipo Grande 2019</p>");
                mensaje.AppendFormat("</div>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("</table>");
                EnviarCorreo(email, "Reinicio de subastas", mensaje.ToString(), img);
            }
            else {
                // ITEM1: IdSubasta:
                // ITEM2: Total de la subasta
                // ITEM3: calidad
                // ITEM4: capacidad
                // ITEM5: fecha inicio de la subasta
                // ITEM6: nombre de usuario
                List<Tuple<int, int, int, int, DateTime?, string>> subastasMenorPrecio = new List<Tuple<int, int, int, int, DateTime?, string>>();
                foreach (var subasta in subastas)
                {
                    var total = subasta.Precio;
                    var calidad = subasta.IsCertificado ? 1 : 0;
                    calidad += subasta.IsRefrigerado ? 1 : 0;
                    var capacidad = subasta.CapacidadCarga;
                    // Caso inicial, en donde no se tiene una ninguna subasta para comparar
                    if (subastasMenorPrecio.Count == 0)
                    {
                        Tuple<int, int, int, int, DateTime?, string> primeraSubasta = new Tuple<int, int, int, int, DateTime?, string>(subasta.Id_subasta, total, calidad, capacidad, subasta.Fecha_inicio, subasta.Username);
                        subastasMenorPrecio.Add(primeraSubasta);
                    }
                    else {
                        Tuple<int, int, int, int, DateTime?, string> menorSubasta = new Tuple<int, int, int, int, DateTime?, string>(-1, -1, -1, -1, null, string.Empty);
                        foreach (var smp in subastasMenorPrecio)
                        {
                            if (smp.Item2 >= total) {
                                menorSubasta = new Tuple<int, int, int, int, DateTime?, string>(subasta.Id_subasta, total, calidad, capacidad, subasta.Fecha_inicio, subasta.Username);
                                break;
                            }
                        }
                        if (menorSubasta.Item1 != -1 && menorSubasta.Item2 != -1) {
                            subastasMenorPrecio.Add(menorSubasta);
                        }
                    }
                }
                // único ganador
                if (subastasMenorPrecio.Count == 1)
                {
                    usuario = su.GetUsuarios().Where(x => x.Username == subastasMenorPrecio.FirstOrDefault().Item6).FirstOrDefault();
                    idSubastaGanadora = subastasMenorPrecio.FirstOrDefault().Item1;
                }
                // buscamos por el siguiente criterio
                else {
                    var subastasMejorCalidad = subastasMenorPrecio.OrderByDescending(x => x.Item3).Where(y => y.Item3 == subastasMenorPrecio.Max(z => z.Item3)).ToList();

                    if (subastasMejorCalidad.Count == 1)
                    {
                        usuario = su.GetUsuarios().Where(x => x.Username == subastasMejorCalidad.FirstOrDefault().Item6).FirstOrDefault();
                        idSubastaGanadora = subastasMejorCalidad.FirstOrDefault().Item1;
                    }
                    else {
                        var primeraSubasta = subastasMejorCalidad.OrderBy(x => x.Item5).FirstOrDefault();
                        usuario = su.GetUsuarios().Where(x => x.Username == primeraSubasta.Item6).FirstOrDefault();
                        idSubastaGanadora = primeraSubasta.Item1;
                    }
                }

                //Cuerpo del correo
                LinkedResource img =
                new LinkedResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\IMAGENES\Frutas.jpg", MediaTypeNames.Image.Jpeg);
                img.ContentId = "imagen";
                StringBuilder mensaje = new StringBuilder();

                mensaje.AppendFormat("<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='padding: 0'>");
                mensaje.AppendFormat("<img style='padding: 0; display: block' src='cid:imagen' width='600px' height='100px'>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("<tr>");
                mensaje.AppendFormat("<td style='background - color: #ecf0f1'>");
                mensaje.AppendFormat("<div style='color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>");
                mensaje.AppendFormat("<h2 style='color: #e67e22; margin: 0 0 7px'>Hola!</h2>");
                mensaje.AppendFormat("<p style='margin: 2px; font - size: 15px'>Su solicitud ya tiene un transportista, cuyos datos son. Nombre: {0}, Apellido: {0}. Ahora su solicitud ha pasado al estado <b>En camino</b>.</p>", usuario.Nombre, usuario.Apellido);
                mensaje.AppendFormat("<p style='color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0'>Maipo Grande 2019</p>");
                mensaje.AppendFormat("</div>");
                mensaje.AppendFormat("</td>");
                mensaje.AppendFormat("</tr>");
                mensaje.AppendFormat("</table>");
                EnviarCorreo(email, "Solicitud se encuentra en camino", mensaje.ToString(), img);
                sv.UpdateVenta(3, idVenta);
                ss.UpdateSubastaGanadora(idSubastaGanadora);
            }

            return usuario;
        }


        /// <summary>
        /// Método que envía correos, si te devuelve true, es por que se ha realizado con éxito
        /// En caso de que devuelva false, es por que se generó algún problema.
        /// </summary>
        /// <returns></returns>
        private bool EnviarCorreo(string correoDestinatario, string asunto, string cuerpo, LinkedResource lr)
        {
            bool r = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.UseDefaultCredentials = false;
                mail.IsBodyHtml = true;
                
                mail.From = new MailAddress("equipomaipogrande@gmail.com");
                mail.To.Add(correoDestinatario);
                mail.Subject = asunto;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                cuerpo, null, "text/html");
                htmlView.LinkedResources.Add(lr);
                mail.AlternateViews.Add(htmlView);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("equipomaipogrande", "duocadmin123");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                r = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                r = false;
            }

            return r;
        }

        public void ComenzarProcesoLocal(int idVentaFinalizada) {

        }
    }
}

