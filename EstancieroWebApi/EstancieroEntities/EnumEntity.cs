using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class EnumEntity
    {
        public enum EstadoPartida
        {
            EnJuego,
            Finalizada,
            Suspendida,
            Pausada,
            Reanudar,
        }
        public enum TipoCasillero
        {
            Inicio,
            Provincia,
            Multa,
            Premio,
        }
        public enum EstadoJugador
        {
            EnJuego,
            Derrotado
        }
        public enum TipoMovimiento
        {
            MovimientoDado,
            CompraProvincia,
            PagoAlquiler,
            Multa,
            Premio
        }
    }
}
