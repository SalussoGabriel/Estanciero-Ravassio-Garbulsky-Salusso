using Estanciero.Data;
using EstancieroEntities;
using EstancieroReponse;
using Newtonsoft.Json;
using static EstancieroEntities.EnumEntity;

namespace EstancieroService
{
    public class PartidaService
    {
        private JugadorService jugador = new JugadorService();
        private PartidaEntity partida = new PartidaEntity();
          public ApiResponse<PartidaResponse> CrearPartida(List<int> dnisJugadores)
          {
              ApiResponse<PartidaResponse> resultado = new ApiResponse<PartidaResponse>();

              if (dnisJugadores.Count < 2 || dnisJugadores.Count > 4)
              {
                  throw new ArgumentException("La partida debe tener entre 2 y 4 jugadores para iniciar.");
              }
              var todos = jugador.ObtenerTodosLosJugadores().Data;
              Console.WriteLine(JsonConvert.SerializeObject(todos, Formatting.Indented));
              foreach (int dni in dnisJugadores)
              {
                  var jugadorConsulta = jugador.ObtenerJugadorPorDni(dni);
                  if (!jugadorConsulta.Success || jugadorConsulta.Data == null)
                  {
                      throw new ArgumentException($"El DNI {dni} no está registrado en el sistema. Regístrelo primero.");
                  }
              }
              int nuevoNumeroPartida = PartidaFile.ObtenerSiguienteNumeroPartida();
              var partida = new PartidaEntity
              {
                  NumeroPartida = nuevoNumeroPartida,
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
                      NumeroPartida = nuevoNumeroPartida,
                      DniJugador = (int)dnisJugadores[i],
                      Posicion = 0,
                      Saldo = 5000000,
                      EstadoJugador = EstadoJugador.EnJuego,
                      HistorialMovimientos = new List<MovimientoEntity>()
                  });
              }
              PartidaFile.EscribirPartida(partida);
              var partidaResponse = new PartidaResponse
              {
                  NumeroPartida = partida.NumeroPartida,
                  EstadoPartida = (EstadoPartidaResponse)partida.EstadoPartida,
                  TurnoActual = partida.TurnoActual,
              };
              resultado.Success = true;
              resultado.Message = "Partida creada con exito";
              resultado.Data = partidaResponse;
              return resultado;
          }
         public ApiResponse<PartidaResponse> ObtenerEstadoPartida(int numeroPartida)
          {
              ApiResponse<PartidaResponse> resultado = new ApiResponse<PartidaResponse>();
              var partidas = PartidaFile.LeerPartidas();
              var partidaBuscada = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
              if (partidaBuscada == null)
              {
                  resultado.Success = false;
                  resultado.Message = "No se encuentra el numero de partida que se ingreso";
                  resultado.Errors.Add("No se encuentra el numero de partida que se ingreso");
                  return resultado;
              }
              int dniTurnoActual = partidaBuscada.ConfiguracionTurnos
                  .FirstOrDefault(c => c.NumeroTurno == partidaBuscada.TurnoActual)?.DniJugador ?? 0;

              var partidaResponse = new PartidaResponse
              {
                  NumeroPartida = partidaBuscada.NumeroPartida,
                  EstadoPartida = (EstadoPartidaResponse)partidaBuscada.EstadoPartida,
                  TurnoActual = dniTurnoActual,
                  DniGanador = partidaBuscada.DniGanador,
                  MotivoGanador = partidaBuscada.MotivoGanador
              };
              resultado.Success = true;
              resultado.Message = "Resultado devuelto con exito";
              resultado.Data = partidaResponse;
              return resultado;
         } 
        public ApiResponse<PartidaResponse> CambiarEstadoPartida(int numeroPartida, bool pausa, bool suspendida, bool reaundar)
        {
            ApiResponse<PartidaResponse> resultado = new ApiResponse<PartidaResponse>();
            var partidas = PartidaFile.LeerPartidas();
            var partidaCambio = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partidaCambio != null)
            {
                if (pausa)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.Pausada;
                }
                else if (suspendida)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.Suspendida;
                    partidaCambio.FechaFin = DateTime.Now; 
                    var ganador = partidaCambio.JugadoresEnPartida .OrderByDescending(j => j.Saldo).FirstOrDefault();
                    if (ganador != null)
                    {
                        partidaCambio.DniGanador = ganador.DniJugador;
                        partidaCambio.MotivoGanador = "Partida suspendida. Ganador por mayor saldo.";
                        resultado.Message += $" Ganador por mayor saldo: {ganador.DniJugador}.";
                    }
                }
                else if (reaundar)
                {
                    partidaCambio.EstadoPartida = EstadoPartida.EnJuego;
                }
                PartidaFile.EscribirPartida(partidaCambio);
                var partidaResponse = new PartidaResponse
                {
                    EstadoPartida = (EstadoPartidaResponse)partidaCambio.EstadoPartida,
                };
                resultado.Success = true;
                resultado.Message = $"Estado de partida actualizado a: {partidaCambio.EstadoPartida.ToString()}";
                resultado.Data = partidaResponse;
                return resultado;
            }
            else 
            {
                resultado.Success = false;
                resultado.Message = "Error: No se encontró ningúna partida con ese numero de partida.";
                resultado.Errors.Add("No se encontró ningúna partida con ese numero de partida.");
                return resultado;
            }
        }
        public ApiResponse<List<PartidaEntity>> ObtenerPartidasPausadas()
        {
            try
            {
                var partidas = PartidaFile.LeerPartidas();
                if (partidas == null)
                {
                    return new ApiResponse<List<PartidaEntity>>
                    {
                        Success = false,
                        Message = "No se pudo acceder a la base de datos de partidas (PartidaFile)."
                    };
                }
                var partidasPausadas = partidas.Where(p => p.EstadoPartida == EstadoPartida.Pausada).ToList();
                if (partidasPausadas.Count == 0)
                {
                    return new ApiResponse<List<PartidaEntity>>
                    {
                        Success = true,
                        Message = "No se encontraron partidas pausadas o suspendidas.",
                        Data = new List<PartidaEntity>()
                    };
                }
                return new ApiResponse<List<PartidaEntity>>
                {
                    Success = true,
                    Message = $"Se encontraron {partidasPausadas.Count} partidas pausadas.",
                    Data = partidasPausadas
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PartidaEntity>>
                {
                    Success = false,
                    Message = "Error interno al obtener partidas pausadas: " + ex.Message
                };
            }
        }
        public ApiResponse<List<JugadorEnPartidaResponse>> ObtenerJugadoresDePartida(int numeroPartida)
        {
            ApiResponse<List<JugadorEnPartidaResponse>> respuesta = new ApiResponse<List<JugadorEnPartidaResponse>>();
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partida == null)
            {
                return new ApiResponse<List<JugadorEnPartidaResponse>> { Success = false, Message = "No se encontró la partida solicitada." };
            }
            var jugadoresActivosEnPartida = partida.JugadoresEnPartida.ToList();
            var todosLosJugadoresSistema = JugadorFile.LeerJugadores() ?? new List<JugadorEntity>();
            if (jugadoresActivosEnPartida.Count == 0)
            {
                respuesta.Success = true;
                respuesta.Message = "Partida encontrada, pero no se encontraron jugadores activos.";
                respuesta.Data = new List<JugadorEnPartidaResponse>();
                return respuesta;
            }
            respuesta.Success = true;
            respuesta.Message = "Jugadores obtenidos con éxito.";
            respuesta.Data = jugadoresActivosEnPartida.Select(j =>
            {
                var jugadorSistema = todosLosJugadoresSistema.FirstOrDefault(sysJ => sysJ.DniJugador == j.DniJugador);
                double saldoFinal = j.Saldo;
                if (j.Saldo == 0.0 && j.Posicion == 0 && (j.HistorialMovimientos == null || j.HistorialMovimientos.Count == 0))
                {
                    saldoFinal = 5000000.0;
                }
                var response = new JugadorEnPartidaResponse
                {
                    DniJugador = j.DniJugador,
                    NombreJugador = jugadorSistema?.NombreJugador ?? "Desconocido",
                    Posicion = j.Posicion,
                    Saldo = saldoFinal, 
                    EstadoJugador = j.EstadoJugador.ToString(),
                };
                return response;
            }).ToList();
            return respuesta;
        }
        public ApiResponse<int?> VictoriaPorCantidadDePropiedades(int numeroPartida, int dniJugador)
        {
            ApiResponse<int?> resultado = new ApiResponse<int?>();
            var busquedaPartida = PartidaFile.LeerPartidas().FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (busquedaPartida == null)
            {
                resultado.Success = false;
                resultado.Message = "Partida no encontrada.";
                return resultado;
            }
            if (busquedaPartida.EstadoPartida == EstadoPartida.Finalizada)
            {
                resultado.Success = false;
                resultado.Message = "La partida ya fue finalizada.";
                return resultado;
            }
            // Contador de las propiedades que tiene el jugador
            int contProvinciasCompradas = busquedaPartida.TableroPartida.Count(c => c.TipoCasillero == TipoCasillero.Provincia && c.DniPropietario == dniJugador);
            if (contProvinciasCompradas >= 1)
            {
                //Console.WriteLine($"[VICTORIA LOGRADA] Partida {numeroPartida}: Jugador {dniJugador} compró la provincia #{contProvinciasCompradas}.");
                busquedaPartida.EstadoPartida = EstadoPartida.Finalizada;
                busquedaPartida.DniGanador = dniJugador;
                busquedaPartida.MotivoGanador = $"Poseer {contProvinciasCompradas} o más provincias compradas.";
                busquedaPartida.FechaFin = DateTime.Now;
                PartidaFile.EscribirPartida(busquedaPartida);
                resultado.Success = true;
                resultado.Message = $"🏆 ¡El jugador {dniJugador} ha ganado la partida por tener {contProvinciasCompradas} provincias!";
                resultado.Data = dniJugador;
            }
            else
            {
                resultado.Success = false;
                resultado.Message = $"El jugador {dniJugador} tiene {contProvinciasCompradas} provincias, aún no gana.";
            }
            return resultado;
        }
        public ApiResponse<int?> VictoriaUnicoJugadorSaldoPositivo(int numeroPartida)
        {
            ApiResponse<int?> resultado = new ApiResponse<int?>();
            var busquedaPartida = PartidaFile.LeerPartidas().FirstOrDefault(p => p.NumeroPartida == numeroPartida);

            if (busquedaPartida == null || busquedaPartida.EstadoPartida != EstadoPartida.EnJuego)
            {
                resultado.Success = false;
                resultado.Message = "Partida no encontrada o no está en juego.";
                return resultado;
            }
            var busquedaJugadoresActivos = busquedaPartida.JugadoresEnPartida.Where(j => j.EstadoJugador == EstadoJugador.EnJuego && j.Saldo > 0).ToList();
            if (busquedaJugadoresActivos.Count == 1)
            {
                int dniGanador = busquedaJugadoresActivos.First().DniJugador;
                busquedaPartida.EstadoPartida = EstadoPartida.Finalizada;
                busquedaPartida.DniGanador = dniGanador;
                busquedaPartida.MotivoGanador = "Único jugador con saldo positivo.";
                PartidaFile.EscribirPartida(busquedaPartida);
                resultado.Success = true;
                resultado.Message = $"¡El jugador {dniGanador} ha ganado por ser el único con saldo positivo!";
                resultado.Data = dniGanador;
            }
            else
            {
                resultado.Success = false;
                resultado.Message = "Aún hay más de un jugador con saldo positivo o todos están en bancarrota.";
            }
            return resultado;
        }

    }
}
