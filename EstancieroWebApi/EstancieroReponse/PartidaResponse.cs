using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroReponse
{
    public enum EstadoPartidaResponse { EnJuego,Finalizada,Suspendida, Pausada }
    public class PartidaResponse
    {
        public int NumeroPartida {  get; set; }
        public EstadoPartidaResponse EstadoPartida { get; set; }
        public int TurnoActual { get; set; }
        public List<JugadorEnPartidaResponse> Jugadores { get; set; }
        public int? DniGanador { get; set; }
        public string? MotivoGanador { get; set; }
    }
}
