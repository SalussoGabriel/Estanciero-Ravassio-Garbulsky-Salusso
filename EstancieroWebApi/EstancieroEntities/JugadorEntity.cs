namespace EstancieroEntities
{
    public class JugadorEntity
    {
        public int DniUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Mail {  get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public EstadisticaJugdor EstadisticasJugador { get; set; }

    }
}
