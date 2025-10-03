using EstancieroReponse;
using EstancieroRequest;
using Estanciero.Data;
using EstancieroEntities;
namespace EstancieroService
{
  public class JugadorService
  {
            public ApiResponse<JugadorResponse> CrearJugador(CrearJugadorRequest request)
            {
                var jugadores = JugadorFile.LeerJugadores();
                bool dniDuplicado = jugadores.Any(j => j.DniJugador == request.DniUsuario);
                bool mailDuplicado = jugadores.Any(j => j.Mail.Equals(request.MailUsuario, StringComparison.OrdinalIgnoreCase));

                if (dniDuplicado || mailDuplicado)
                { 
                    return new ApiResponse<JugadorResponse>
                    {
                        Success = false,
                        Message = "Error: Ya existe un jugador con ese DNI o Correo Electrónico."
                    };
                }
                var nuevoJugador = new JugadorEntity
                {
                    DniJugador = request.DniUsuario,
                    NombreJugador = request.NombreUsuario,
                    Mail = request.MailUsuario,
                    FechaRegistro = DateTime.Now,
                };
                JugadorFile.EscribirJugador(nuevoJugador);
                var jugadorResponse = new JugadorResponse
                {
                    DniUsuario = nuevoJugador.DniJugador,
                    NombreUsuario = nuevoJugador.NombreJugador,
                };
                return new ApiResponse<JugadorResponse>
                {
                    Success = true,
                    Message = "Jugador creado correctamente.",
                    Data = jugadorResponse
                };
            }
            public ApiResponse<JugadorResponse> ObtenerJugadorPorDni(int dniJugador)
            {
              var jugadores = JugadorFile.LeerJugadores();
              var buscarJugador = jugadores.FirstOrDefault(d => d.DniJugador == dniJugador);
              if (buscarJugador == null)
              {
                return new ApiResponse<JugadorResponse>
                {
                    Success = false,
                    Message = "Jugador no encontrado"
                };
              }
              var jugadorResponse = new JugadorResponse
              {
                DniUsuario = buscarJugador.DniJugador,
                NombreUsuario = buscarJugador.NombreJugador,
                PartidasGanadas = buscarJugador.EstadisticasJugador.PartidasGanadas,
                PartidasPerdidas = buscarJugador.EstadisticasJugador.PartidasPerdidas,
                PartidasJugadas = buscarJugador.EstadisticasJugador.PartidasJugadas,
              };
               return new ApiResponse<JugadorResponse>
               {
                Success = true,
                Message = "Este en el jugador pedido con el dni.",
                Data = jugadorResponse
               };

            }
        public void ActualizarEstadisticasJugador(int dniJugador, bool gano, bool derrotado, bool pendiente)
        {
            var jugadores = JugadorFile.LeerJugadores();
            var jugadorAct = jugadores.FirstOrDefault(d => d.DniJugador == dniJugador);
            if (jugadorAct != null)
            {
                jugadorAct.EstadisticasJugador.PartidasJugadas++;
                if (gano)
                {
                    jugadorAct.EstadisticasJugador.PartidasGanadas++;
                } else if (derrotado)
                {
                    jugadorAct.EstadisticasJugador.PartidasPerdidas++;
                } else if (pendiente)
                {
                    jugadorAct.EstadisticasJugador.PartidasPendientes++;
                }
                JugadorFile.EscribirJugador(jugadorAct);
            }

        }
  }
}

