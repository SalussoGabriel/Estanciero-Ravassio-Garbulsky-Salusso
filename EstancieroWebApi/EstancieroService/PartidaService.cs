using Estanciero.Data;
using EstancieroEntities;
using EstancieroReponse;
using static EstancieroEntities.EnumEntity;

namespace EstancieroService
{
    public class PartidaService
    {
        public PartidaEntity CrearPartida(List<int> dnisJugadores)
        {
            if (dnisJugadores.Count < 2 || dnisJugadores.Count > 4)
            {
                throw new ArgumentException("La partida debe tener entre 2 y 4 jugadores");
            }
            var partida = new PartidaEntity
            {
                FechaInicio = DateTime.Now,
                TurnoActual = 1,
                EstadoPartida = EstadoPartida.EnJuego,
                ConfiguracionTurnos = new List<ConfiguracionTurnos>(),
                TableroPartida = TableroFile.LeerTablero(),
                JugadoresEnPartida = new List<JugadorEnPartidaEntity>()
            };
            for (int i = 0; i < dnisJugadores.Count; i++)
            {
                partida.ConfiguracionTurnos.Add(new ConfiguracionTurnos
                {
                    NumeroTurno = i + 1,
                    DniJugador = dnisJugadores[i]
                });
                partida.JugadoresEnPartida.Add(new JugadorEnPartidaEntity
                {
                    NumeroPartida = partida.NumeroPartida,
                    DniJugador = partida.DniUsuario,
                    Posicion = 0,
                    Saldo = 5000000,
                    EstadoJugador = EstadoJugador.EnJuego,
                    HistorialMovimientos = new List<MovimientoEntity>()
                });
            }
            PartidaFile.EscribirPartida(partida);
            return partida;
        }
        public ApiResponse<PartidaResponse> ObtenerEstadoPartida(int numeroPartida)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partidaBuscada = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partidaBuscada == null)
            {
                return new ApiResponse<PartidaResponse>
                {
                    Success = false,
                    Message = "No se encuentra el numero de partida que se ingreso"
                };                
            }
            var partidaResponse = new PartidaResponse
            {
                NumeroPartida = partidaBuscada.NumeroPartida,
                EstadoPartida = (EstadoPartidaResponse)partidaBuscada.EstadoPartida,
                TurnoActual = partidaBuscada.TurnoActual,
                //Ver tema de jugadores en partida como devolverlo
            };
            return new ApiResponse<PartidaResponse>
            {
                Success = true,
                Message = "Resultado devuelto con exito",
                Data = partidaResponse
            };
        }
        public void CambiarEstadoPartida(int numeroPartida, bool pausa, bool suspendida, bool reaundar)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partidaCambio = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partidaCambio != null)
            {
                if (pausa)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.Pausada;
                } else if (suspendida)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.Suspendida;
                } else if (reaundar)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.Re;
                }
                PartidaFile.EscribirPartida(partidaCambio);
            }
        }
        
    }
}
