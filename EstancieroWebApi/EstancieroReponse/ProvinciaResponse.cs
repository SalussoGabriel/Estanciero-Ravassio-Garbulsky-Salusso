namespace EstancieroReponse
{
    public class ProvinciaResponse
    {
        public int NroCasillero { get; set; }
        public string NombreProvincia { get; set; }  // CORRECCIÓN: debe ser string
        public int? DniPropietario { get; set; }     // puede ser nulo si está disponible
    }
}
