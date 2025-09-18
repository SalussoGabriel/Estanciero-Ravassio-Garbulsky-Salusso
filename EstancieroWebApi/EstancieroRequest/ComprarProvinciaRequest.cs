using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroRequest
{
    public class ComprarProvinciaRequest
    {
        [Required]
        public int IdPartida {  get; set; }
        [Required(ErrorMessage ="Dni obligatorio")]
        public int DniUsuario { get; set; }
        [Required]
        public int NroCasillero { get; set; }

    }
}
