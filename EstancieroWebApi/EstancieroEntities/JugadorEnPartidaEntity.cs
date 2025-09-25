using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EstancieroEntities.EnumEntity;

namespace EstancieroEntities
{
    public class JugadorEnPartidaEntity
    {
        public int NroPartida { get; set; }
        public int DniJugador { get; set; }
        public int Posicion { get; set; }
        public decimal Saldo { get; set; }
        public EstadoJugador EstadoJugador { get; set; } = EstadoJugador.EnJuego;
        public List<MovimientoEntity> HistorialMovimientos { get; set; }

        public JugadorEnPartidaEntity()
        { 
           HistorialMovimientos = new List<MovimientoEntity>();
        }
    }
}
