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
        public int NumeroPartida { get; set; }
        public int DniJugador { get; set; }
        public string NombreJugador { get; set; }
        public int Posicion { get; set; }
        public double Saldo { get; set; }
        public EstadoJugador EstadoJugador { get; set; } = EstadoJugador.EnJuego;
        public List<MovimientoEntity> HistorialMovimientos { get; set; }

        public JugadorEnPartidaEntity()
        { 
           HistorialMovimientos = new List<MovimientoEntity>();
        }
    }
}
