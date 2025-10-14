using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroRequest
{
    public class CrearPartidaRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(4)]
        public List<int> DniJugadores { get; set; }
    }
}
