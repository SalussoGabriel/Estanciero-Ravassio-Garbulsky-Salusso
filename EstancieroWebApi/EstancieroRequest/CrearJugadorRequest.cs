using System.ComponentModel.DataAnnotations;

namespace EstancieroRequest
{
    public class CrearJugadorRequest
    {
        [Required(ErrorMessage ="El dni del usuario es un campo obligatorio")]
        [Range(8,8, ErrorMessage = "El dni de usuario debe ser mayor a 0")]
        public int DniUsuario {  get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage ="El mail es un campo obligatorio")]
        [EmailAddress(ErrorMessage ="El mail no cumple con las caracteristicas")]
        public string MailUsuario { get; set; }


    }
}
