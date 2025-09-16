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
        public int IdPartida {  get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin {  get; set; }
        public int Duracion { get; set; }
        public EstadoPartida EstadoJugador { get; set; }
        public int DniUsuario { get; set; }
        public string Detalle {  get; set; }
        public int NroCasillero { get; set; }
        public bool GanadorSIoNO { get; set; }
        public string MotivoGanador { get; set; }

    }
}
