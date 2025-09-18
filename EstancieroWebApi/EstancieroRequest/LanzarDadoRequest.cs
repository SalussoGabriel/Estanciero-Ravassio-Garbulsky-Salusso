using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroRequest
{
    public class LanzarDadoRequest
    {
        [Required(ErrorMessage = "Campo obligatorio")]
        public int DniUsuario {  get; set; }
        [Required]
        public int IdPartida { get; set; }


    }
}
