using System.ComponentModel.DataAnnotations;

namespace EstancieroRequest
{
    public class CrearJugadorRequest
    {
        [Required(ErrorMessage ="El dni del jugador es un campo obligatorio")]
        public int DniJugador {  get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string NombreJugador { get; set; }
        [Required(ErrorMessage ="El mail es un campo obligatorio")]
        [EmailAddress(ErrorMessage ="El mail no cumple con las caracteristicas")]
        public string MailJugador { get; set; }
    }
}
