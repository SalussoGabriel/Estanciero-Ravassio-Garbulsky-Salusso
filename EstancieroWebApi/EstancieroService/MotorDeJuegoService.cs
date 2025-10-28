using Estanciero.Data;
using EstancieroEntities;
using EstancieroReponse;
using System;
using System.Linq;
using System.Collections.Generic;
using static EstancieroEntities.EnumEntity;

namespace EstancieroService
{
    public class MotorDeJuegoService
    {
        public int ConsultarTurnoActual(int numeroPartida)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partida == null)
                throw new ArgumentException("Partida no encontrada");
            var turno = partida.ConfiguracionTurnos.FirstOrDefault(c => c.NumeroTurno == partida.TurnoActual);
            return turno != null ? turno.DniJugador : 0;
        }
        public int AvanzarTurno(int numeroPartida)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partida == null)
                throw new ArgumentException("Partida no encontrada");
            if (partida.ConfiguracionTurnos == null || partida.ConfiguracionTurnos.Count == 0)
                throw new InvalidOperationException("Configuración de turnos vacía");
            // Calcula el índice del siguiente turno (1, 2, 3...)
            int indiceSiguienteTurno = (partida.TurnoActual % partida.ConfiguracionTurnos.Count) + 1;
            partida.TurnoActual = indiceSiguienteTurno;
            // Busca el DNI del jugador cuyo turno comienza
            int dniSiguienteTurno = partida.ConfiguracionTurnos
                .FirstOrDefault(c => c.NumeroTurno == indiceSiguienteTurno)?.DniJugador ?? 0;
            PartidaFile.EscribirPartida(partida);
            // Devuelve el DNI del jugador para el JSON.
            return dniSiguienteTurno;
        }
        public ApiResponse<string> MoverJugador(int numeroPartida, int dniJugador)
        {
            ApiResponse<string> resultado = new ApiResponse<string>();
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partida == null)
            {
                resultado.Success = false;
                resultado.Message = "No se encontró la partida.";
                return resultado;
            }
            if (partida.EstadoPartida == EstadoPartida.Finalizada)
            {
                resultado.Success = false;
                resultado.Message = "La partida ya ha finalizado.";
                return resultado;
            }
            var jugador = partida.JugadoresEnPartida.FirstOrDefault(j => j.DniJugador == dniJugador);
            if (jugador == null)
            {
                resultado.Success = false;
                resultado.Message = "No se encontró el jugador en la partida.";
                return resultado;
            }
            // Lanzar dado dentro del método
            int valorDado = LanzarDado();
            // Calcular nueva posición
            int nuevaPosicion = (jugador.Posicion + valorDado) % 31;
            // Obtener casillero destino
            var casilleroDestino = partida.TableroPartida?.FirstOrDefault(c => c.NroCasillero == nuevaPosicion);
            if (casilleroDestino == null)
            {
                if (partida.TableroPartida == null || partida.TableroPartida.Count == 0)
                {
                    partida.TableroPartida = GenerarTableroDefault();
                    casilleroDestino = partida.TableroPartida.FirstOrDefault(c => c.NroCasillero == nuevaPosicion);
                }
                if (casilleroDestino == null)
                {
                    resultado.Success = false;
                    resultado.Message = "Casillero destino no encontrado (tablero no inicializado correctamente)";
                    return resultado;
                }
            }
            // Actualizar posición del jugador
            jugador.Posicion = nuevaPosicion;
            var mensajeCasillero = AplicarReglasCasillero(jugador, casilleroDestino, partida);
            PartidaService partidaService = new PartidaService();
            var checkVictoriaProps = partidaService.VictoriaPorCantidadDePropiedades(partida.NumeroPartida, jugador.DniJugador);
            var checkVictoriaSaldo = partidaService.VictoriaUnicoJugadorSaldoPositivo(partida.NumeroPartida);
            var partidaActualizada = PartidaFile.LeerPartidas().FirstOrDefault(p => p.NumeroPartida == numeroPartida);
            if (partidaActualizada != null && partidaActualizada.EstadoPartida != EstadoPartida.Finalizada)
            {
                PartidaFile.EscribirPartida(partida);
            }
            resultado.Success = true;
            resultado.Message = $"El jugador {jugador.NombreJugador} avanzó {valorDado} posiciones hasta {casilleroDestino.NombreProvincia}. {mensajeCasillero}";
            if (checkVictoriaProps.Success)
            {
                resultado.Message += $" | {checkVictoriaProps.Message}";
            }
            else if (checkVictoriaSaldo.Success)
            {
                resultado.Message += $" | {checkVictoriaSaldo.Message}";
            }
            return resultado;
        }
        private int LanzarDado()
        {
            // Instanciar Random una vez por llamada está OK para el uso del juego simple.
            Random random = new Random();
            return random.Next(1, 7); // 1..6
        }
        private string AplicarReglasCasillero(JugadorEnPartidaEntity jugador, CasilleroTableroEntity casillero, PartidaEntity partida)
        {
            string mensaje = "";
            switch (casillero.TipoCasillero)
            {
                case TipoCasillero.Provincia:
                    {
                        double? precioCompra = casillero.PrecioProvincia;
                        double? precioAlquiler = casillero.PrecioAlquilerProvincia;
                        if (!casillero.DniPropietario.HasValue || casillero.DniPropietario.Value == 0) 
                        {
                            // El jugador puede comprar sólo si tiene saldo suficiente y si existe precio
                            if (precioCompra.HasValue && jugador.Saldo >= Convert.ToDouble(precioCompra.Value))
                            {
                                // Comprar provincia
                                jugador.Saldo -= Convert.ToDouble(precioCompra.Value);
                                // Asignación del DNI 
                                casillero.DniPropietario = jugador.DniJugador;
                                mensaje = $"Provincia '{casillero.NombreProvincia}' comprada por ${precioCompra.Value:N0}";
                                var movimientoCompra = new MovimientoEntity
                                {
                                    IdMovimiento = jugador.HistorialMovimientos.Count + 1,
                                    Fecha = DateTime.Now,
                                    Tipo = TipoMovimiento.CompraProvincia,
                                    Descripcion = $"Compra de provincia '{casillero.NombreProvincia}'",
                                    Monto = precioCompra.HasValue ? precioCompra.Value : 0,
                                    CasilleroOrigen = jugador.Posicion,
                                    CasilleroDestino = jugador.Posicion,
                                    DniJugadorAfectado = jugador.DniJugador
                                };
                                jugador.HistorialMovimientos.Add(movimientoCompra);
                            }
                            else
                            {
                                mensaje = $"No tienes dinero suficiente para comprar '{casillero.NombreProvincia}' (${(precioCompra ?? 0):N0})";
                            }
                        }
                        else if (casillero.DniPropietario.HasValue && casillero.DniPropietario.Value == jugador.DniJugador)
                        {
                            mensaje = $"Caíste en tu propia provincia: {casillero.NombreProvincia}";
                        }
                        else
                        {
                            // Pagar alquiler al propietario
                            if (!precioAlquiler.HasValue)
                            {
                                mensaje = $"Provincia '{casillero.NombreProvincia}' no tiene monto de alquiler definido.";
                            }
                            else
                            {
                                double montoAlquilerDec = Convert.ToDouble(precioAlquiler.Value);
                                // Restar al jugador
                                jugador.Saldo -= montoAlquilerDec;
                                // Acreditar al propietario si está en partida
                                var propietario = partida.JugadoresEnPartida.FirstOrDefault(j => j.DniJugador == casillero.DniPropietario.Value);
                                if (propietario != null)
                                {
                                    propietario.Saldo += montoAlquilerDec;
                                }
                                mensaje = $"Alquiler pagado por '{casillero.NombreProvincia}': ${precioAlquiler.Value:N0}";
                                var movimientoAlquiler = new MovimientoEntity
                                {
                                    IdMovimiento = jugador.HistorialMovimientos.Count + 1,
                                    Fecha = DateTime.Now,
                                    Tipo = TipoMovimiento.PagoAlquiler,
                                    Descripcion = $"Pago de alquiler por '{casillero.NombreProvincia}'",
                                    Monto = precioAlquiler.Value,
                                    CasilleroOrigen = jugador.Posicion,
                                    CasilleroDestino = jugador.Posicion,
                                    DniJugadorAfectado = jugador.DniJugador
                                };
                                jugador.HistorialMovimientos.Add(movimientoAlquiler);
                            }
                        }
                    }
                    break;
                case TipoCasillero.Multa:
                    {
                        double montoMulta = casillero.Monto ?? 0;
                        jugador.Saldo -= Convert.ToDouble(montoMulta);
                        mensaje = $"Multa aplicada: ${montoMulta:N0}";
                        var movimientoMulta = new MovimientoEntity
                        {
                            IdMovimiento = jugador.HistorialMovimientos.Count + 1,
                            Fecha = DateTime.Now,
                            Tipo = TipoMovimiento.Multa,
                            Descripcion = $"Multa aplicada",
                            Monto = montoMulta,
                            CasilleroOrigen = jugador.Posicion,
                            CasilleroDestino = jugador.Posicion,
                            DniJugadorAfectado = jugador.DniJugador
                        };
                        jugador.HistorialMovimientos.Add(movimientoMulta);
                    }
                    break;
                case TipoCasillero.Premio:
                    {
                        double montoPremio = casillero.Monto ?? 0;
                        jugador.Saldo += Convert.ToDouble(montoPremio);
                        mensaje = $"¡Premio recibido! +${montoPremio:N0}";
                        var movimientoPremio = new MovimientoEntity
                        {
                            IdMovimiento = jugador.HistorialMovimientos.Count + 1,
                            Fecha = DateTime.Now,
                            Tipo = TipoMovimiento.Premio,
                            Descripcion = $"Premio recibido",
                            Monto = montoPremio,
                            CasilleroOrigen = jugador.Posicion,
                            CasilleroDestino = jugador.Posicion,
                            DniJugadorAfectado = jugador.DniJugador
                        };
                        jugador.HistorialMovimientos.Add(movimientoPremio);
                    }
                    break;

                case TipoCasillero.Inicio:
                    mensaje = "Caíste en el casillero de inicio";
                    break;

                default:
                    mensaje = "Casillero sin reglas aplicables";
                    break;
            }
            if (jugador.Saldo <= 0)
            {
                jugador.EstadoJugador = EstadoJugador.Derrotado;
                mensaje += " | ¡BANCARROTA! El jugador ha sido derrotado";
            }
            return mensaje;
        }
        private List<CasilleroTableroEntity> GenerarTableroDefault()
        {
            return new List<CasilleroTableroEntity>
     {
         new CasilleroTableroEntity { NroCasillero = 0, NombreProvincia = "Salida", TipoCasillero = TipoCasillero.Inicio },
         new CasilleroTableroEntity { NroCasillero = 1, NombreProvincia = "Buenos Aires", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 300000, PrecioAlquilerProvincia = 25000 },
         new CasilleroTableroEntity { NroCasillero = 2, NombreProvincia = "Catamarca", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 120000, PrecioAlquilerProvincia = 10000 },
         new CasilleroTableroEntity { NroCasillero = 3, NombreProvincia = "Chaco", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 150000, PrecioAlquilerProvincia = 12000 },
         new CasilleroTableroEntity { NroCasillero = 4, TipoCasillero = TipoCasillero.Multa, Monto = 5000 },
         new CasilleroTableroEntity { NroCasillero = 5, NombreProvincia = "Chubut", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 180000, PrecioAlquilerProvincia = 15000 },
         new CasilleroTableroEntity { NroCasillero = 6, NombreProvincia = "Córdoba", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 250000, PrecioAlquilerProvincia = 20000 },
         new CasilleroTableroEntity { NroCasillero = 7, NombreProvincia = "Corrientes", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 160000, PrecioAlquilerProvincia = 13000 },
         new CasilleroTableroEntity { NroCasillero = 8, TipoCasillero = TipoCasillero.Multa, Monto = 10000 },
         new CasilleroTableroEntity { NroCasillero = 9, NombreProvincia = "Entre Ríos", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 170000, PrecioAlquilerProvincia = 14000 },
         new CasilleroTableroEntity { NroCasillero = 10, NombreProvincia = "Formosa", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 130000, PrecioAlquilerProvincia = 11000 },
         new CasilleroTableroEntity { NroCasillero = 11, TipoCasillero = TipoCasillero.Premio, Monto = 50000 },
         new CasilleroTableroEntity { NroCasillero = 12, NombreProvincia = "Jujuy", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 140000, PrecioAlquilerProvincia = 12000 },
         new CasilleroTableroEntity { NroCasillero = 13, NombreProvincia = "La Pampa", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 150000, PrecioAlquilerProvincia = 13000 },
         new CasilleroTableroEntity { NroCasillero = 14, TipoCasillero = TipoCasillero.Multa, Monto = 15000 },
         new CasilleroTableroEntity { NroCasillero = 15, NombreProvincia = "La Rioja", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 125000, PrecioAlquilerProvincia = 10000 },
         new CasilleroTableroEntity { NroCasillero = 16, NombreProvincia = "Mendoza", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 220000, PrecioAlquilerProvincia = 18000 },
         new CasilleroTableroEntity { NroCasillero = 17, NombreProvincia = "Misiones", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 160000, PrecioAlquilerProvincia = 13000 },
         new CasilleroTableroEntity { NroCasillero = 18, TipoCasillero = TipoCasillero.Premio, Monto = 100000 },
         new CasilleroTableroEntity { NroCasillero = 19, NombreProvincia = "Neuquén", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 200000, PrecioAlquilerProvincia = 17000 },
         new CasilleroTableroEntity { NroCasillero = 20, NombreProvincia = "Río Negro", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 190000, PrecioAlquilerProvincia = 16000 },
         new CasilleroTableroEntity { NroCasillero = 21, TipoCasillero = TipoCasillero.Multa, Monto = 20000 },
         new CasilleroTableroEntity { NroCasillero = 22, NombreProvincia = "Salta", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 140000, PrecioAlquilerProvincia = 12000 },
         new CasilleroTableroEntity { NroCasillero = 23, NombreProvincia = "San Juan", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 180000, PrecioAlquilerProvincia = 15000 },
         new CasilleroTableroEntity { NroCasillero = 24, NombreProvincia = "San Luis", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 170000, PrecioAlquilerProvincia = 14000 },
         new CasilleroTableroEntity { NroCasillero = 25, TipoCasillero = TipoCasillero.Multa, Monto = 25000 },
         new CasilleroTableroEntity { NroCasillero = 26, NombreProvincia = "Santa Cruz", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 200000, PrecioAlquilerProvincia = 17000 },
         new CasilleroTableroEntity { NroCasillero = 27, NombreProvincia = "Santa Fe", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 240000, PrecioAlquilerProvincia = 20000 },
         new CasilleroTableroEntity { NroCasillero = 28, NombreProvincia = "Santiago del Estero", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 150000, PrecioAlquilerProvincia = 12000 },
         new CasilleroTableroEntity { NroCasillero = 29, NombreProvincia = "Tierra del Fuego", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 210000, PrecioAlquilerProvincia = 18000 },
         new CasilleroTableroEntity { NroCasillero = 30, NombreProvincia = "Tucumán", TipoCasillero = TipoCasillero.Provincia, PrecioProvincia = 180000, PrecioAlquilerProvincia = 15000 }
     };
        }
    }
}
    
       
   


