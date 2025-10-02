using EstancieroEntities;
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
                ConfiguracionTurnos = new List<ConfiguracionTurno>(),
                //Falta cargar tablero ; VER
            };
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
    }
}
