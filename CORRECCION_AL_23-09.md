# CORRECCIÓN AL 23-09

## Resumen de la Sesión
- **Fecha**: 23 de septiembre de 2025
- **Repositorios analizados**: Estanciero-Ravassio-Garbulsky-Salusso
- **Enfoque principal**: Revisión de modelado de datos, estructura de proyectos y lógica de negocio básica

## Correcciones Realizadas

### Repositorio: Estanciero-Ravassio-Garbulsky-Salusso

#### Archivo: EstancieroWebApi/EstancieroData/Usuario.cs - Línea 7
**Problema identificado**: Faltan propiedades de estadísticas según el enunciado
**Corrección aplicada**: Agregar propiedades faltantes y crear clase de estadísticas
**Código original**:
```csharp
public int CantidadPartidasGanadas { get; set; }
public int CantidadPartidasPerdidas { get; set; }
```

**Código corregido**:
```csharp
public DateTime FechaRegistro { get; set; }
public DateTime? FechaEliminacion { get; set; }
public EstadisticasJugador Estadisticas { get; set; }

public class EstadisticasJugador
{
    public int PartidasJugadas { get; set; }
    public int PartidasGanadas { get; set; }
    public int PartidasPerdidas { get; set; }
    public int PartidasPendientes { get; set; }
}
```

**Explicación técnica**: El enunciado especifica que los usuarios deben tener un módulo de estadísticas que incluya partidas jugadas, ganadas, perdidas y pendientes. Además, se requieren fechas de registro y eliminación para auditoría.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 7
**Problema identificado**: El ID de partida debería ser NumeroPartida según el enunciado
**Corrección aplicada**: Cambiar nombre de propiedad
**Código original**:
```csharp
public int IdPartida { get; set; }
```

**Código corregido**:
```csharp
public int NumeroPartida { get; set; }
```

**Explicación técnica**: El enunciado especifica que las partidas se definen con un "número de partida", no un ID genérico.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 9
**Problema identificado**: FechaFin debería ser nullable ya que las partidas pueden estar en curso
**Corrección aplicada**: Hacer FechaFin nullable
**Código original**:
```csharp
public DateTime FechaFin { get; set; }
```

**Código corregido**:
```csharp
public DateTime? FechaFin { get; set; }
```

**Explicación técnica**: Las partidas en curso no tienen fecha de fin, por lo que esta propiedad debe ser nullable.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 11
**Problema identificado**: Falta la propiedad Duracion según el enunciado
**Corrección aplicada**: Agregar propiedad Duracion
**Código original**:
```csharp
public DateTime? FechaFin { get; set; }
public EstadoPartida EstadoPartida { get; set; }
```

**Código corregido**:
```csharp
public DateTime? FechaFin { get; set; }
public TimeSpan? Duracion { get; set; }
public EstadoPartida EstadoPartida { get; set; }
```

**Explicación técnica**: El enunciado especifica que las partidas tienen una duración que se setea al finalizar la partida.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 12
**Problema identificado**: TurnoPartida debería ser TurnoActual (int) según el enunciado
**Corrección aplicada**: Cambiar a TurnoActual como entero
**Código original**:
```csharp
public Turno TurnoPartida { get; set; }
```

**Código corregido**:
```csharp
public int TurnoActual { get; set; }
```

**Explicación técnica**: El enunciado especifica que las partidas tienen un "turno actual" como número entero, no como objeto Turno.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 13
**Problema identificado**: UsuarioGanador debería ser DniGanador (int) según el enunciado
**Corrección aplicada**: Cambiar a DniGanador como int
**Código original**:
```csharp
public Usuario UsuarioGanador { get; set; }
```

**Código corregido**:
```csharp
public int? DniGanador { get; set; }
```

**Explicación técnica**: El enunciado especifica que cada partida debe registrar el ganador por DNI, no como objeto Usuario completo.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 14
**Problema identificado**: MotivoVictoria debería ser string según el enunciado
**Corrección aplicada**: Cambiar a string
**Código original**:
```csharp
public Motivo MotivoVictoria { get; set; }
```

**Código corregido**:
```csharp
public string? MotivoVictoria { get; set; }
```

**Explicación técnica**: El enunciado especifica que se debe registrar el motivo de victoria como texto descriptivo.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 15
**Problema identificado**: ListadoTurnos debería ser ConfiguracionTurnos según el enunciado
**Corrección aplicada**: Cambiar nombre y estructura
**Código original**:
```csharp
public List<Turno> ListadoTurnos { get; set; }
```

**Código corregido**:
```csharp
public List<ConfiguracionTurno> ConfiguracionTurnos { get; set; }
public List<CasilleroTablero> Tablero { get; set; }

public class ConfiguracionTurno
{
    public int NumeroTurno { get; set; }
    public int DniJugador { get; set; }
}
```

**Explicación técnica**: El enunciado especifica que las partidas tienen una "configuración de turnos" que especifica por cada turno el número de turno y el DNI del jugador. También se requiere el tablero de casilleros.

---

#### Archivo: EstancieroWebApi/EstancieroData/Partida.cs - Línea 16
**Problema identificado**: Falta el estado "Pausada" según el enunciado
**Corrección aplicada**: Agregar estado Pausada
**Código original**:
```csharp
public enum EstadoPartida { EnJuego, Finalizada, Suspendida }
```

**Código corregido**:
```csharp
public enum EstadoPartida { EnJuego, Finalizada, Suspendida, Pausada }
```

**Explicación técnica**: El enunciado especifica que el sistema debe permitir pausar/reanudar partidas, por lo que se requiere el estado "Pausada".

---

#### Archivo: EstancieroWebApi/EstancieroData/Casillero.cs - Línea 7
**Problema identificado**: El nombre de la clase debería ser CasilleroTablero según el enunciado
**Corrección aplicada**: Cambiar nombre de clase
**Código original**:
```csharp
public class Casillero
```

**Código corregido**:
```csharp
public class CasilleroTablero
```

**Explicación técnica**: Para mayor claridad y consistencia con el enunciado, la clase debe llamarse CasilleroTablero.

---

#### Archivo: EstancieroWebApi/EstancieroData/Casillero.cs - Línea 9
**Problema identificado**: IdCasillero debería ser NroCasillero según el enunciado
**Corrección aplicada**: Cambiar nombre de propiedad
**Código original**:
```csharp
public int IdCasillero { get; set; }
```

**Código corregido**:
```csharp
public int NroCasillero { get; set; }
```

**Explicación técnica**: El enunciado especifica que los casilleros tienen un "número de casillero", no un ID genérico.

---

#### Archivo: EstancieroWebApi/EstancieroData/Casillero.cs - Línea 14
**Problema identificado**: UsuarioPropietario debería ser DniPropietario (int) según el enunciado
**Corrección aplicada**: Cambiar a DniPropietario como int nullable
**Código original**:
```csharp
public Usuario UsuarioPropietario { get; set; }
```

**Código corregido**:
```csharp
public int? DniPropietario { get; set; }
```

**Explicación técnica**: El enunciado especifica que los casilleros tienen un "DNI del propietario actual (nulo si está disponible para adquirir)".

---

#### Archivo: EstancieroWebApi/EstancieroData/Casillero.cs - Línea 15
**Problema identificado**: MontoCasillero debería ser Monto según el enunciado
**Corrección aplicada**: Cambiar nombre de propiedad
**Código original**:
```csharp
public double MontoCasillero { get; set; }
```

**Código corregido**:
```csharp
public double? Monto { get; set; }
```

**Explicación técnica**: El enunciado especifica que los casilleros tienen un "monto" para multas y premios. Debe ser nullable ya que no todos los casilleros tienen monto.

---

#### Archivo: EstancieroWebApi/EstancieroData/Casillero.cs - Línea 16
**Problema identificado**: TipoCasillero incompleto según el enunciado
**Corrección aplicada**: Completar tipos de casillero
**Código original**:
```csharp
public enum TipoCasillero { Inicio, Provincia, Multa, CobroPremio}
```

**Código corregido**:
```csharp
public enum TipoCasillero { Inicio, Provincia, Multa, Premio }
```

**Explicación técnica**: El enunciado especifica que hay casilleros de multas y premios del banco, no "CobroPremio".

---

#### Archivo: EstancieroWebApi/EstancieroData/Movimiento.cs - Línea 7
**Problema identificado**: Estructura incorrecta según el enunciado
**Corrección aplicada**: Reestructurar completamente
**Código original**:
```csharp
public class Movimiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public string Descripcion { get; set; }
    public double Monto { get; set; }
    public int CasilleroOrigen { get; set; }
    public int CasilleroDestino { get; set; }
    public int? DniJugadorAfectado { get; set; }
}
```

**Código corregido**:
```csharp
public class PartidaDetalle
{
    public int NumeroPartida { get; set; }
    public int DniJugador { get; set; }
    public int PosicionActual { get; set; }
    public double DineroDisponible { get; set; }
    public EstadoUsuario Estado { get; set; }
    public List<Movimiento> HistorialMovimientos { get; set; }
}

public class Movimiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public string Descripcion { get; set; }
    public double Monto { get; set; }
    public int CasilleroOrigen { get; set; }
    public int CasilleroDestino { get; set; }
    public int? DniJugadorAfectado { get; set; }
}

public enum TipoMovimiento
{
    MovimientoDado,
    CompraProvincia,
    PagoAlquiler,
    Multa,
    Premio
}
```

**Explicación técnica**: El enunciado especifica que cada partida debe registrar el historial de movimientos (compra de provincias, pagos de alquiler, multas, premios). Se requiere tanto PartidaDetalle como Movimiento.

---

#### Archivo: EstancieroWebApi/EstancieroService/PartidaService.cs - Línea 8
**Problema identificado**: Método incompleto y sin implementación
**Corrección aplicada**: Implementar lógica básica
**Código original**:
```csharp
public Partida CrearPartida(List<Usuario> listadoJugadores)
{
    // instanciar partida
    // elegir los turnos
    if (listadoJugadores.Count <= 4 && listadoJugadores.Count >= 2)
    {
        Partida partida = new Partida()
        {

        };
    }
    return new Partida();
}
```

**Código corregido**:
```csharp
public Partida CrearPartida(List<int> dnisJugadores)
{
    if (dnisJugadores.Count < 2 || dnisJugadores.Count > 4)
    {
        throw new ArgumentException("La partida debe tener entre 2 y 4 jugadores");
    }

    var partida = new Partida
    {
        NumeroPartida = GenerarNumeroPartida(),
        FechaInicio = DateTime.Now,
        EstadoPartida = EstadoPartida.EnJuego,
        TurnoActual = 1,
        ConfiguracionTurnos = new List<ConfiguracionTurno>(),
        Tablero = CargarTablero()
    };

    // Configurar turnos
    for (int i = 0; i < dnisJugadores.Count; i++)
    {
        partida.ConfiguracionTurnos.Add(new ConfiguracionTurno
        {
            NumeroTurno = i + 1,
            DniJugador = dnisJugadores[i]
        });
    }

    return partida;
}

private int GenerarNumeroPartida()
{
    // Lógica para generar número único de partida
    return new Random().Next(1000, 9999);
}

private List<CasilleroTablero> CargarTablero()
{
    // Lógica para cargar configuración del tablero desde archivo JSON
    return new List<CasilleroTablero>();
}
```

**Explicación técnica**: El método debe implementar la lógica completa para crear una partida según el enunciado, incluyendo validaciones, configuración de turnos y carga del tablero.

---

#### ARCHIVO NO ENCONTRADO: EstancieroWebApi/EstancieroData/Archivos.cs
**Problema identificado**: No existe implementación de persistencia en archivos JSON
**Corrección aplicada**: Implementar gestión de archivos JSON
**Archivos a crear**:
- Archivos.cs (con métodos para leer/escribir JSON)
- configuracionTablero.json

**Explicación técnica**: Según el enunciado, la persistencia debe ser en archivos JSON.

---

## Recomendaciones Generales
- Implementar la arquitectura completa de 6 proyectos según la referencia
- Agregar validaciones con Data Annotations en las clases Request
- Implementar manejo de errores consistente
- Crear archivos JSON de configuración del tablero
- Implementar todos los endpoints de la API según el enunciado

## Conceptos Teóricos Aplicados
- **Unidad 1**: POO - Encapsulación, herencia y polimorfismo en el modelado de entidades
- **Unidad 4**: Manejo de archivos JSON para persistencia de datos
- **Unidad 5**: Arquitectura REST con separación de responsabilidades en capas
- **Unidad 5**: Request/Response pattern para API REST

## Próximos Pasos Sugeridos
- Completar la implementación de la lógica de negocio en los servicios
- Crear los proyectos faltantes (Data con Archivos.cs)
- Implementar la persistencia en archivos JSON
- Desarrollar el frontend HTML/CSS/JavaScript

## Resumen de Avance
**Lo que está bien:**
- Estructura de proyectos bien definida
- Entidades básicas creadas
- Algunos Request/Response implementados
- README con información del equipo

**Lo que falta/está mal:**
- Modelado de datos incorrecto según enunciado
- Lógica de negocio no implementada
- No hay persistencia en archivos JSON
- Falta implementación de Archivos.cs
- No hay frontend

**Grado de avance: 25%** - Se tiene una buena estructura de proyectos y algunas entidades, pero falta la implementación completa según el enunciado.