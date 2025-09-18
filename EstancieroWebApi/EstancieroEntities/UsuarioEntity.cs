namespace EstancieroEntities
{
    public class UsuarioEntity
    {
        public int DniUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Mail {  get; set; }
        public List<string> PartidasJugadas { get; set; }

        public UsuarioEntity()
        {
            PartidasJugadas = new List<string>();
        }
    }
}
