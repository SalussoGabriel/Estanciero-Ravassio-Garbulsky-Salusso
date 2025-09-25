using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EstancieroEntities.EnumEntity;

namespace EstancieroEntities
{
    public class CasilleroTableroEntity
    {
        public int NroCasillero { get; set; }
        public TipoCasillero TipoCasillero { get; set; }
        public string NombreProvincia { get; set; }
        public double? PrecioProvincia {  get; set; }
        public double? PrecioAlquilerProvincia { get; set; }
        public int? DniPropietario { get; set; }

    }
}
