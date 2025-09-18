using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroReponse
{
    public enum EstadoPartidaResponse { EnJuego,Finalizada,Suspendida }
    public class PartidaResponse
    {
        public int IdPartida {  get; set; }
        public EstadoPartidaResponse EstadoPartida { get; set; }
        public int TurnoActual { get; set; }

        public List<UsuariosEnPartidaResponse> Jugadores { get; set; }
    }
}
