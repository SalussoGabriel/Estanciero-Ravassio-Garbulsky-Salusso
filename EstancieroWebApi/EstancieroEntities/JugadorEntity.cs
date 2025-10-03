namespace EstancieroEntities
{
    public class JugadorEntity
    {
        public int DniJugador { get; set; }
        public string NombreJugador { get; set; }
        public string Mail {  get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public EstadisticaJugdor EstadisticasJugador { get; set; }

    }
}
