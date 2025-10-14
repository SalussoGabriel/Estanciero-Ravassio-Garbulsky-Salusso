# CORRECCIONES AL 12-10

## Resumen de la Sesión
- **Fecha**: 12 de octubre de 2025
- **Repositorio analizado**: Estanciero-Ravassio-Garbulsky-Salusso
- **Enfoque principal**: Evaluación de lógica de negocio implementada en segunda versión según hitos del 07/10

## Correcciones Realizadas

### Repositorio: Estanciero-Ravassio-Garbulsky-Salusso

#### ❌ NO IMPLEMENTADO: Método para consultar turno actual
**Archivo**: `EstancieroService/MotorDeJuegoService.cs` - Línea 9-13
**Estado**: ❌ **NO IMPLEMENTADO**
**Análisis**: La clase `MotorDeJuegoService` está completamente vacía. No se implementó ningún método para consultar el turno actual.

**Corrección sugerida**:
```csharp
public class MotorDeJuegoService
{
    public int ConsultarTurnoActual(int numeroPartida)
    {
        var partidas = PartidaFile.LeerPartidas();
        var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
        
        if (partida == null)
        {
            throw new ArgumentException("Partida no encontrada");
        }
        
        return partida.TurnoActual;
    }
    
    public void AvanzarTurno(int numeroPartida)
    {
        var partidas = PartidaFile.LeerPartidas();
        var partida = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
        
        if (partida == null)
        {
            throw new ArgumentException("Partida no encontrada");
        }
        
        int siguienteTurno = (partida.TurnoActual % partida.ConfiguracionTurnos.Count) + 1;
        partida.TurnoActual = siguienteTurno;
        
        PartidaFile.EscribirPartida(partida);
    }
}
```

---

#### ❌ NO IMPLEMENTADO: Método para mover el jugador y aplicar reglas automáticamente según el casillero
**Archivo**: `EstancieroService/MotorDeJuegoService.cs` - Línea 9-13
**Estado**: ❌ **NO IMPLEMENTADO**
**Análisis**: No se implementó ningún método para mover jugadores y aplicar reglas de casilleros.

**Corrección sugerida**:
```csharp
public class MotorDeJuegoService
{
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
        
        // Lanzar dado
        int dado = LanzarDado();
        
        // Calcular nueva posición
        int nuevaPosicion = (jugador.Posicion + dado) % 31;
        if (nuevaPosicion == 0) nuevaPosicion = 31; // Vuelta al inicio
        
        // Obtener casillero destino
        var casilleroDestino = partida.TableroPartida.FirstOrDefault(c => c.NroCasillero == nuevaPosicion);
        if (casilleroDestino == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Casillero no encontrado"
            };
        }
        
        // Aplicar reglas del casillero
        string mensaje = AplicarReglasCasillero(jugador, casilleroDestino, partida);
        
        // Actualizar posición del jugador
        jugador.Posicion = nuevaPosicion;
        
        // Crear movimiento
        var movimiento = new MovimientoEntity
        {
            Id = jugador.HistorialMovimientos.Count + 1,
            DniJugadorAfectado = dniJugador,
            CasilleroOrigen = jugador.Posicion - dado,
            CasilleroDestino = nuevaPosicion,
            Fecha = DateTime.Now,
            Descripcion = $"Movimiento con dado: {dado}",
            Tipo = TipoMovimiento.MovimientoDado
        };
        
        jugador.HistorialMovimientos.Add(movimiento);
        
        // Guardar cambios
        PartidaFile.EscribirPartida(partida);
        
        return new ApiResponse<string>
        {
            Success = true,
            Message = $"Jugador movido a casillero {nuevaPosicion}. {mensaje}"
        };
    }
    
    private int LanzarDado()
    {
        Random random = new Random();
        return random.Next(1, 7);
    }
}
```

---

#### ❌ NO IMPLEMENTADO: Gestionar compra de propiedades
**Archivo**: `EstancieroService/MotorDeJuegoService.cs` - Línea 9-13
**Estado**: ❌ **NO IMPLEMENTADO**
**Análisis**: No se implementó ningún método para gestionar la compra de propiedades.

**Corrección sugerida**:
```csharp
private string AplicarReglasCasillero(JugadorEnPartidaEntity jugador, CasilleroTableroEntity casillero, PartidaEntity partida)
{
    string mensaje = "";
    
    switch (casillero.TipoCasillero)
    {
        case TipoCasillero.Provincia:
            if (casillero.DniPropietario == null)
            {
                // Verificar si el jugador tiene dinero suficiente
                if (jugador.Saldo >= casillero.PrecioCompra)
                {
                    // Comprar provincia
                    jugador.Saldo -= casillero.PrecioCompra;
                    casillero.DniPropietario = jugador.DniJugador;
                    mensaje = $"Provincia '{casillero.NombreProvincia}' comprada por ${casillero.PrecioCompra:N0}";
                    
                    // Crear movimiento de compra
                    var movimientoCompra = new MovimientoEntity
                    {
                        Id = jugador.HistorialMovimientos.Count + 1,
                        DniJugadorAfectado = jugador.DniJugador,
                        CasilleroOrigen = jugador.Posicion,
                        CasilleroDestino = jugador.Posicion,
                        Fecha = DateTime.Now,
                        Descripcion = $"Compra de provincia '{casillero.NombreProvincia}'",
                        Tipo = TipoMovimiento.CompraProvincia,
                        Monto = casillero.PrecioCompra
                    };
                    jugador.HistorialMovimientos.Add(movimientoCompra);
                }
                else
                {
                    mensaje = $"No tienes dinero suficiente para comprar '{casillero.NombreProvincia}' (${casillero.PrecioCompra:N0})";
                }
            }
            else if (casillero.DniPropietario == jugador.DniJugador)
            {
                mensaje = $"Caíste en tu propia provincia: {casillero.NombreProvincia}";
            }
            else
            {
                // Pagar alquiler
                jugador.Saldo -= casillero.PrecioAlquiler;
                mensaje = $"Alquiler pagado por '{casillero.NombreProvincia}': ${casillero.PrecioAlquiler:N0}";
                
                // Crear movimiento de alquiler
                var movimientoAlquiler = new MovimientoEntity
                {
                    Id = jugador.HistorialMovimientos.Count + 1,
                    DniJugadorAfectado = jugador.DniJugador,
                    CasilleroOrigen = jugador.Posicion,
                    CasilleroDestino = jugador.Posicion,
                    Fecha = DateTime.Now,
                    Descripcion = $"Pago de alquiler por '{casillero.NombreProvincia}'",
                    Tipo = TipoMovimiento.PagoAlquiler,
                    Monto = casillero.PrecioAlquiler
                };
                jugador.HistorialMovimientos.Add(movimientoAlquiler);
            }
            break;
            
        case TipoCasillero.Multa:
            jugador.Saldo -= casillero.Monto;
            mensaje = $"Multa aplicada: ${casillero.Monto:N0}";
            
            // Crear movimiento de multa
            var movimientoMulta = new MovimientoEntity
            {
                Id = jugador.HistorialMovimientos.Count + 1,
                DniJugadorAfectado = jugador.DniJugador,
                CasilleroOrigen = jugador.Posicion,
                CasilleroDestino = jugador.Posicion,
                Fecha = DateTime.Now,
                Descripcion = $"Multa aplicada",
                Tipo = TipoMovimiento.Multa,
                Monto = casillero.Monto
            };
            jugador.HistorialMovimientos.Add(movimientoMulta);
            break;
            
        case TipoCasillero.Premio:
            jugador.Saldo += casillero.Monto;
            mensaje = $"¡Premio recibido! +${casillero.Monto:N0}";
            
            // Crear movimiento de premio
            var movimientoPremio = new MovimientoEntity
            {
                Id = jugador.HistorialMovimientos.Count + 1,
                DniJugadorAfectado = jugador.DniJugador,
                CasilleroOrigen = jugador.Posicion,
                CasilleroDestino = jugador.Posicion,
                Fecha = DateTime.Now,
                Descripcion = $"Premio recibido",
                Tipo = TipoMovimiento.Premio,
                Monto = casillero.Monto
            };
            jugador.HistorialMovimientos.Add(movimientoPremio);
            break;
            
        case TipoCasillero.Inicio:
            mensaje = "Caíste en el casillero de inicio";
            break;
    }
    
    // Verificar si el jugador está en bancarrota
    if (jugador.Saldo <= 0)
    {
        jugador.EstadoJugador = EstadoJugador.Derrotado;
        mensaje += " | ¡BANCARROTA! El jugador ha sido derrotado";
    }
    
    return mensaje;
}
```

---

#### ❌ NO IMPLEMENTADO: Calcular e imputar alquileres
**Archivo**: `EstancieroService/MotorDeJuegoService.cs` - Línea 9-13
**Estado**: ❌ **NO IMPLEMENTADO**
**Análisis**: No se implementó la lógica para calcular e imputar alquileres. Esta funcionalidad debería estar en el método `AplicarReglasCasillero()`.

**Corrección sugerida**: Ver implementación en el método `AplicarReglasCasillero()` arriba.

---

#### ❌ NO IMPLEMENTADO: Administrar multas y premios
**Archivo**: `EstancieroService/MotorDeJuegoService.cs` - Línea 9-13
**Estado**: ❌ **NO IMPLEMENTADO**
**Análisis**: No se implementó la lógica para administrar multas y premios. Esta funcionalidad debería estar en el método `AplicarReglasCasillero()`.

**Corrección sugerida**: Ver implementación en el método `AplicarReglasCasillero()` arriba.

---

## Correcciones Adicionales Identificadas

### Archivo: `EstancieroService/PartidaService.cs` - Línea 35
**Problema identificado**: Error en asignación de DNI
**Corrección sugerida**:
```csharp
// Código actual (incorrecto):
DniJugador = partida.DniUsuario,

// Código corregido:
DniJugador = dnisJugadores[i],
```

### Archivo: `EstancieroService/PartidaService.cs` - Línea 85
**Problema identificado**: Error en estado de partida
**Corrección sugerida**:
```csharp
// Código actual (incorrecto):
partidaCambio.EstadoPartida = EstadoPartida.Re;

// Código corregido:
partidaCambio.EstadoPartida = EstadoPartida.EnJuego;
```

### Archivo: `EstancieroService/PartidaService.cs` - Línea 21
**Problema identificado**: Tipo incorrecto en configuración de turnos
**Corrección sugerida**:
```csharp
// Código actual (incorrecto):
ConfiguracionTurnos = new List<ConfiguracionTurnos>(),

// Código corregido:
ConfiguracionTurnos = new List<ConfiguracionTurno>(),
```

---

## Evaluación General

### ❌ **NO COMPLETADO** - Hitos del 07/10
- ❌ Método para consultar turno actual (no implementado)
- ❌ Método para mover el jugador y aplicar reglas automáticamente según el casillero (no implementado)
- ❌ Gestionar compra de propiedades (no implementado)
- ❌ Calcular e imputar alquileres (no implementado)
- ❌ Administrar multas y premios (no implementado)

### 🟡 **PARCIALMENTE COMPLETADO**
- Estructura básica de proyectos
- Persistencia en archivos JSON
- Algunos métodos de gestión de partidas

### ❌ **FALTANTE**
- Lógica de negocio del motor de juego
- API REST funcional
- Frontend HTML/CSS/JavaScript
- Validaciones de entrada
- Manejo de errores
- Tests unitarios

## Conceptos Teóricos Aplicados Parcialmente
- **Unidad 1**: POO - Encapsulación básica implementada
- **Unidad 4**: Manejo de archivos JSON implementado
- **Unidad 5**: Arquitectura REST no implementada correctamente
- **Unidad 5**: Request/Response pattern implementado parcialmente

## Recomendaciones para Próximos Pasos
1. **Implementar completamente** la clase `MotorDeJuegoService`
2. **Corregir errores** en la clase `PartidaService`
3. **Implementar API REST** con controladores apropiados
4. **Agregar validaciones** con Data Annotations
5. **Implementar frontend** HTML/CSS/JavaScript
6. **Agregar manejo de errores** consistente

## Resumen de Avance
**Lo que está bien:**
- ✅ Estructura de proyectos creada
- ✅ Persistencia en archivos JSON implementada
- ✅ Entidades definidas
- ✅ Request/Response pattern implementado

**Lo que necesita corrección:**
- ❌ Motor de juego no implementado
- ❌ Lógica de negocio faltante
- ❌ Errores en PartidaService
- ❌ No hay API REST funcional
- ❌ No hay frontend

**Grado de avance: 30%** - Se tiene la estructura básica y persistencia, pero falta la implementación completa de la lógica de negocio según los hitos del 07/10.
