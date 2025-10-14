using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroReponse
{
    public class UsuariosEnPartidaResponse
    {
        public int DniJugador { get; set; }
        public string NombreJugador { get; set; }
        public int Posicion { get; set; }
        public decimal Saldo { get; set; }
        public string EstadoJugador { get; set; }

    }
}
