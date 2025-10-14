using Estanciero.Data;
using EstancieroEntities;
using EstancieroReponse;
using EstancieroRequest;
using System.ComponentModel;
using System.Net;
using System.Runtime.Intrinsics.X86;
namespace EstancieroService
{
    public class JugadorService
    {
        public ApiResponse<JugadorResponse> CrearJugador(CrearJugadorRequest request)
        {
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            var jugadores = JugadorFile.LeerJugadores();
            bool dniDuplicado = jugadores.Any(j => j.DniJugador == request.DniJugador);
            bool mailDuplicado = jugadores.Any(j => j.Mail.Equals(request.MailJugador, StringComparison.OrdinalIgnoreCase));

            if (dniDuplicado || mailDuplicado)
            {
                resultado.Success = false;
                resultado.Message = "Error: Ya existe un jugador con ese DNI o Correo Electrónico.";
                resultado.Errors.Add("Ya existe un jugador con ese DNI o Correo Electrónico.");
                return resultado;
            }
            var nuevoJugador = new JugadorEntity
            {
                DniJugador = request.DniJugador,
                NombreJugador = request.NombreJugador,
                Mail = request.MailJugador,
                FechaRegistro = DateTime.Now,
            };
            JugadorFile.EscribirJugador(nuevoJugador);
            var jugadorResponse = new JugadorResponse
            {
                DniJugador = nuevoJugador.DniJugador,
                NombreJugador = nuevoJugador.NombreJugador,
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
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            var jugadores = JugadorFile.LeerJugadores();
            var buscarJugador = jugadores.FirstOrDefault(d => d.DniJugador == dniJugador);
            if (buscarJugador == null)
            {
                resultado.Success = false;
                resultado.Message = "Jugador no encontrado";
                resultado.Errors.Add("Jugador no encontrado");
            }
            if (buscarJugador.EstadisticasJugador == null)
            {
                buscarJugador.EstadisticasJugador = new EstadisticaJugdor(); //Esta linea esta hecha porque al probar con swagger decia que EstadisticasJugador devolvia null
            }
            var jugadorResponse = new JugadorResponse
            {
                DniJugador = buscarJugador.DniJugador,
                NombreJugador = buscarJugador.NombreJugador,
                PartidasGanadas = buscarJugador.EstadisticasJugador.PartidasGanadas,
                PartidasPerdidas = buscarJugador.EstadisticasJugador.PartidasPerdidas,
                PartidasPendientes = buscarJugador.EstadisticasJugador.PartidasPendientes,
                PartidasJugadas = buscarJugador.EstadisticasJugador.PartidasJugadas,
            };
            return new ApiResponse<JugadorResponse>
            {
                Success = true,
                Message = "Este en el jugador pedido con el dni.",
                Data = jugadorResponse
            };
        }
        public ApiResponse<JugadorResponse> ActualizarEstadisticasJugador(int dniJugador, bool gano, bool derrotado, bool pendiente)
        {
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            var jugadores = JugadorFile.LeerJugadores();
            var jugadorAct = jugadores.FirstOrDefault(d => d.DniJugador == dniJugador);
            if (jugadorAct != null)
            {
                if (jugadorAct.EstadisticasJugador == null)
                {
                    jugadorAct.EstadisticasJugador = new EstadisticaJugdor();
                }
                jugadorAct.EstadisticasJugador.PartidasJugadas++;
                if (gano)
                {
                    jugadorAct.EstadisticasJugador.PartidasGanadas++;
                }
                else if (derrotado)
                {
                    jugadorAct.EstadisticasJugador.PartidasPerdidas++;
                }
                else if (pendiente)
                {
                    jugadorAct.EstadisticasJugador.PartidasPendientes++;
                }
                JugadorFile.EscribirJugador(jugadorAct);
                // Correcion de mapeo
                var jugadorResponse = new JugadorResponse
                {
                    DniJugador = jugadorAct.DniJugador,
                    NombreJugador = jugadorAct.NombreJugador,
                    PartidasGanadas = jugadorAct.EstadisticasJugador.PartidasGanadas,
                    PartidasPerdidas = jugadorAct.EstadisticasJugador.PartidasPerdidas,
                    PartidasPendientes = jugadorAct.EstadisticasJugador.PartidasPendientes,
                    PartidasJugadas = jugadorAct.EstadisticasJugador.PartidasJugadas,
                };
                resultado.Success = true;
                resultado.Message = "Estadísticas actualizadas y guardadas correctamente.";
                resultado.Data = jugadorResponse; 

                return resultado;
            }
            resultado.Success = false;
            resultado.Message = "Error: No se encontró ningún jugador con ese DNI.";
            resultado.Errors.Add("DNI no registrado o inexistente.");
            return resultado;
        }
    }
}

