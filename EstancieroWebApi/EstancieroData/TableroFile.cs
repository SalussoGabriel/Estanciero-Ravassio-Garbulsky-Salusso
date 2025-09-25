using EstancieroEntities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Estanciero.Data
{
    public class TableroFile
    {
        public static string archivo = Path.GetFullPath("datos/tablero.json");

        public static List<CasilleroTableroEntity> LeerTablero()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<CasilleroTableroEntity>>(json);
            }
            else
            {
                return new List<CasilleroTableroEntity>();
            }
        }
    }
}

