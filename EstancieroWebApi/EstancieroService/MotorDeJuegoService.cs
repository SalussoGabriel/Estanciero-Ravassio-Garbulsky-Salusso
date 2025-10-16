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

            return partida.TurnoActual;
        }

        public void AvanzarTurno(int numeroPartida)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);

            if (partida == null)
                throw new ArgumentException("Partida no encontrada");

            if (partida.ConfiguracionTurnos == null || partida.ConfiguracionTurnos.Count == 0)
                throw new InvalidOperationException("Configuración de turnos vacía");

            int siguienteTurno = (partida.TurnoActual % partida.ConfiguracionTurnos.Count) + 1;
            partida.TurnoActual = siguienteTurno;

            PartidaFile.EscribirPartida(partida);
        }

        public ApiResponse<string> MoverJugador(int numeroPartida, int dniJugador)
        {
            var partidas = PartidaFile.LeerPartidas();
            var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);

            if (partida == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Partida no encontrada"
                };
            }

            var jugador = partida.JugadoresEnPartida.FirstOrDefault(j => j.DniJugador == dniJugador);
            if (jugador == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Jugador no encontrado en la partida"
                };
            }

            if (partida.EstadoPartida == EstadoPartida.Pausada)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "La partida está pausada"
                };
            }

            // Lanzar dado
            int dado = LanzarDado();

            // Calcular nueva posición (tablero de 0 a 30 -> módulo 31)
            int origen = jugador.Posicion;
            int nuevaPosicion = (jugador.Posicion + dado) % 31;

            // Obtener casillero destino
            var casilleroDestino = partida.TableroPartida.FirstOrDefault(c => c.NroCasillero == nuevaPosicion);
            if (casilleroDestino == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Casillero destino no encontrado"
                };
            }

            // Aplicar reglas del casillero (incluye compras, multas, premios, alquileres)
            string mensajeReglas = AplicarReglasCasillero(jugador, casilleroDestino, partida);

            // Actualizar posición del jugador
            jugador.Posicion = nuevaPosicion;

            // Crear movimiento de tipo MovimientoDado
            var movimiento = new MovimientoEntity
            {
                IdMovimiento = jugador.HistorialMovimientos.Count + 1,
                Fecha = DateTime.Now,
                Tipo = TipoMovimiento.MovimientoDado,
                Descripcion = $"Movimiento con dado: {dado}",
                Monto = 0,
                CasilleroOrigen = origen,
                CasilleroDestino = nuevaPosicion,
                DniJugadorAfectado = jugador.DniJugador
            };

            jugador.HistorialMovimientos.Add(movimiento);

            // Guardar cambios
            PartidaFile.EscribirPartida(partida);

            return new ApiResponse<string>
            {
                Success = true,
                Message = $"Jugador movido a casillero {nuevaPosicion}. {mensajeReglas}"
            };
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
                        // Precio y alquiler pueden ser null en la definición; cojo valores con seguridad
                        double? precioCompra = casillero.PrecioProvincia;
                        double? precioAlquiler = casillero.PrecioAlquilerProvincia;

                        if (!casillero.DniPropietario.HasValue)
                        {
                            // El jugador puede comprar sólo si tiene saldo suficiente y si existe precio
                            if (precioCompra.HasValue && jugador.Saldo >= Convert.ToDecimal(precioCompra.Value))
                            {
                                // Comprar provincia
                                jugador.Saldo -= Convert.ToDecimal(precioCompra.Value);
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
                        else if (casillero.DniPropietario == jugador.DniJugador)
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
                                decimal montoAlquilerDec = Convert.ToDecimal(precioAlquiler.Value);

                                // Restar al jugador
                                jugador.Saldo -= montoAlquilerDec;

                                // Acreditar al propietario si está en partida
                                var propietario = partida.JugadoresEnPartida.FirstOrDefault(j => j.DniJugador == casillero.DniPropietario);
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
                        double montoMulta = casillero.Monto.HasValue ? casillero.Monto.Value : 0; //ERROR EN MONTO
                        jugador.Saldo -= Convert.ToDecimal(montoMulta);
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
                        double montoPremio = casillero.Monto.HasValue ? casillero.Monto.Value : 0; //ERROR EN MONTO
                        jugador.Saldo += Convert.ToDecimal(montoPremio);
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

            // Verificar bancarrota
            if (jugador.Saldo <= 0)
            {
                jugador.EstadoJugador = EstadoJugador.Derrotado;
                mensaje += " | ¡BANCARROTA! El jugador ha sido derrotado";
            }

            // Si después de las acciones hay que verificar ganador por condiciones (12 provincias, único con saldo positivo, etc.)
            // No lo implemento automáticamente a menos que quieras que el motor determine ganador aquí.
            // Guardado de la partida lo hacemos en el método público que llamó a esto.

            return mensaje;
        }
    }
}

