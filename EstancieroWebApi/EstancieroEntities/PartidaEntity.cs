using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EstancieroEntities.EnumEntity;

namespace EstancieroEntities
{
    public class PartidaEntity
    {
        public int NumeroPartida {  get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin {  get; set; }
        public TimeSpan? Duracion { get; set; }
        public EstadoPartida EstadoPartida { get; set; }
        public EstadoPartida EstadoJugador { get; set; }
        public int DniUsuario { get; set; }
        public string Detalle {  get; set; }
        public int TurnoActual { get; set; }
        public List<CasilleroTableroEntity> ListadoCasilleros { get; set; }
        public List<JugadorEnPartidaEntity> UsuariosEnPartida { get; set; }
        public List<ConfiguracionTurno> ConfiguracionTurnos { get; set; }
        public bool GanadorSIoNO { get; set; }
        public int? DniGanador { get; set;  }
        public string? MotivoGanador { get; set; }

        public PartidaEntity()
        {
            ListadoCasilleros = new List<CasilleroTableroEntity>();
            UsuariosEnPartida = new List<JugadorEnPartidaEntity>();
            ConfiguracionTurnos = new List<ConfiguracionTurno>();
        }
        

    }
}
