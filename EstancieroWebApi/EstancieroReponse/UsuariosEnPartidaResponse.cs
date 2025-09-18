using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroReponse
{
    public class UsuariosEnPartidaResponse
    {
        public int DniUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int Posicion { get; set; }
        public decimal Saldo { get; set; }
        public string EstadoUsuario { get; set; }

    }
}
