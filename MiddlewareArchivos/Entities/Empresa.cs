using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Entities
{
    internal class Empresa
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool ManejaSecuencial { get; set; }
        public string MetodoSalida { get; set; }
        public string WebhookSecret { get; set; }
    }
}
